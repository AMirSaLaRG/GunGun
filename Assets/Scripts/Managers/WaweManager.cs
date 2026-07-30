using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaweManager : MonoBehaviour
{
    private RespawnManager respawnManager;

  
    [Header ("Setup")]

    [SerializeField] private List<WaveData> waweData;
    //private List<WaveData> waweData = new List<WaveData>();
    private bool isStarted = false;

    private bool isWaweEnded = true;
    private bool isTimeForNextWawe = true;
    private int currentWaveIndex = 0;

    private float waweStartTime;

    private List<LevelEventSequenceSo> sqs = new List<LevelEventSequenceSo>();
    private List<float> timeMarksForSqs = new List<float>();

    private bool taskIsSq;
    private int currentSqsIndex = 0;


    private void Start()
    {
        ResetWaweManager();

        List<WaveData> refrenceSqData = new List<WaveData>();

        foreach (var data in waweData)
        {
            if (data.Type == WaveType.EventBase || data.eventSequenceSo != null)
            {
                sqs.Add(data.eventSequenceSo);
                refrenceSqData.Add(data);
            }
        }

        foreach (var d in refrenceSqData)
        {
            waweData.Remove(d);
        }

        foreach (var sq in sqs)
        {
            timeMarksForSqs.Add(sq.startTime);
        }
    }

    private void Update()
    {
        if (isStarted == false)
            return;

        if (isTimeForEvent())
            ExecuteSequence();


        StartWawe();
    }

    public void StartWaves()
    {
        isStarted = true;

        isWaweEnded = true;
        isTimeForNextWawe = true;

        waweStartTime = Time.time;
    }

    public void StopWaves()
    {
        ClearScene();
        respawnManager.BreakRespawn();
    }
    private void ResetWaweManager()
    {
        currentWaveIndex = 0;
        currentSqsIndex = 0;
        isStarted = false;
    }
    private bool isTimeForEvent()
    {

        if (sqs.Count == 0)
            return false;

        if (currentSqsIndex >= sqs.Count)
            return false;

        return timeMarksForSqs[currentSqsIndex] < (Time.time - waweStartTime);
    }

    private void ExecuteSequence()
    {
        Debug.Log("Executing Event");
        LevelEventSequenceSo currentSq = sqs[currentSqsIndex];
        currentSqsIndex++;

        if (currentSq.shouldOtherEventsGoTORest)
            respawnManager.BreakRespawn();

        isWaweEnded = false;
        isTimeForNextWawe = false;
        taskIsSq = true;

        StartCoroutine(BreathTimeBeforeEvent(currentSq));
    }

    private IEnumerator BreathTimeBeforeEvent(LevelEventSequenceSo sq)
    {

        yield return new WaitForSeconds(2);

        respawnManager.ExecuteEvent(sq);

        respawnManager.onEventEnded += OnEventEnded;

    }

    private void OnEventEnded()
    {
        isWaweEnded = true;
        isTimeForNextWawe = true;
        taskIsSq = false;
    }

    private void StartWawe()
    {
        if (isWaweEnded == false)
            return;

        if (isTimeForNextWawe == false)
            return;

        if (taskIsSq)
            return;

        if (waweData == null || waweData.Count == 0)
        {
    
            Debug.Log("Plz write Wawe Data first!");
            return;
        }

        if (currentWaveIndex == waweData.Count)
        {
            if (respawnManager.isSceenClear)
            {
                GameManager.instance.LevelCompleted();
                isWaweEnded = true;
                isTimeForNextWawe = false;
            }

            return;
        }

        ExecuteWawe(waweData[currentWaveIndex]);
    }

    private void ExecuteWawe(WaveData data)
    {
        CleanData(data);

        isWaweEnded = false;
        isTimeForNextWawe = false;
        currentWaveIndex++;      

        respawnManager.SetupRandom(data.respawns, data.ActiveBoxes, 
            data.minMaxRandomIntervalSummon, data.minMaxNumSummon, 
            data.respawnNumIfRespawnBase, data.respawnTypeIfRespawnBase);

        if (data.Type == WaveType.TimeBase)
        {

            SetTimerForWawe(data.durationIfTimeBase);
        }

    }

    private void SetTimerForWawe(float durationIfTimeBase)
    {
        float duration = durationIfTimeBase;
        StartCoroutine(DurationCo(duration));
    }

    
    private IEnumerator DurationCo(float duration)
    {
        yield return new WaitForSeconds(duration);
        respawnManager.BreakRespawn();
        isWaweEnded = true;
        StartCoroutine(BreathTimeBetweenTwoWawes());
    }
    private void CleanData(WaveData data)
    {
        CleanRepeat(data);
        CleanProbs(data);

    }

    private static void CleanRepeat(WaveData data)
    {
        var group = data.respawns.GroupBy(x => x.respawnType).Select(group => new RespawnData
        {
            respawnType = group.Key,
            respawnProb = group.Average(x => x.respawnProb),
            prefab = group.First().prefab,
        }) .ToList();


        data.respawns.Clear();
        data.respawns.AddRange(group);

       
    }
    private void RespawnManagerTaskOver()
    {
        isWaweEnded = true;

        StartCoroutine(BreathTimeBetweenTwoWawes());
    }

    private IEnumerator BreathTimeBetweenTwoWawes()
    {
        float duration = 0f;
        if (taskIsSq)
            duration = 2f;
        else
            duration = waweData[currentWaveIndex - 1].nextWaweDelay;
        

        yield return new WaitForSeconds(duration);
        isTimeForNextWawe = true;
        taskIsSq = false;
    }

    private static void CleanProbs(WaveData data)
    {
        List<float> probs = new List<float>();

        foreach (RespawnData rData in data.respawns)
            probs.Add(rData.respawnProb);

        float sumProbs = probs.Sum();

        for (int i = 0; i < probs.Count; i++)
            data.respawns[i].respawnProb = data.respawns[i].respawnProb / sumProbs;
    }

    public void ClearScene()
    {
        respawnManager.BreakRespawn();

        ResetWaweManager();

        foreach (var target in FindObjectsByType<Target>(FindObjectsSortMode.InstanceID))
        {
            target.DOKill();
            Destroy(target.gameObject);
        }
        foreach (var target in FindObjectsByType<EnemyProjectal>(FindObjectsSortMode.InstanceID))
        {
            target.DOKill();
            Destroy(target.gameObject);
        }

        StopAllCoroutines();
    }
    public void SetRespawnManager(RespawnManager respawnManager)
    {
        this.respawnManager = respawnManager;
        this.respawnManager.jobDone += RespawnManagerTaskOver;
    }
}
