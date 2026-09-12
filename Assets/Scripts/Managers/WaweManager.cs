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
    private PlayerController player;
    private ScoreManager scoreManager;
    private LevelManager myManager;

  
    [Header ("Setup")]

    [SerializeField] private List<WaveData> wawesData;
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

    private bool isSaftyAtEndTriggered = false;

    private AudioSource waveChangeSfx;

    private void Awake()
    {
        respawnManager = FindFirstObjectByType<RespawnManager>();
        player = FindFirstObjectByType<PlayerController>();
        scoreManager = FindFirstObjectByType<ScoreManager>();
    }

    private void Start()
    {
        ResetWaweManager();

        List<WaveData> refrenceSqData = new List<WaveData>();

        foreach (var data in wawesData)
        {
            if (data.Type == WaveType.EventBase || data.eventSequenceSo != null)
            {
                sqs.Add(data.eventSequenceSo);
                refrenceSqData.Add(data);
            }
        }

        foreach (var d in refrenceSqData)
        {
            wawesData.Remove(d);
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

        if (respawnManager.isSceenClear)
            if (CheckToExecuteWave())
                ExecuteWawe(wawesData[currentWaveIndex]);

    }

    private void ExecuteWawe(WaveData data)
    {
        CleanData(data);

        isWaweEnded = false;
        isTimeForNextWawe = false;

        respawnManager.SetupRandom(data);
        string waveNum = GetCurrentWaveText();
        if (currentWaveIndex == 0)
            AudioManager.instance.PlaySfx(myManager.GetLevelStartSfx());
        else if (currentWaveIndex == wawesData.Count - 1)
            AudioManager.instance.PlaySfx(myManager.GetFinalWaveSfx());
        else
            AudioManager.instance.PlaySfx(myManager.GetWaveChangeSfx());

        player.WaveChange(data, waveNum);
        scoreManager.ExtandComboTime(data.startingDelay);


        if (data.Type == WaveType.TimeBase)
            SetTimerForWawe(data);

    }

    private string GetCurrentWaveText()
    {
        string waveNum = (currentWaveIndex + 1).ToString();
        if (currentWaveIndex == wawesData.Count - 1)
            waveNum = "FINAL";
        if (currentWaveIndex == 0)
            waveNum = "START";
        return waveNum;
    }

    private void SetTimerForWawe(WaveData data)
    {
        StartCoroutine(DurationCo(data.durationIfTimeBase + data.startingDelay));
    }


    private IEnumerator DurationCo(float duration)
    {
        yield return new WaitForSeconds(duration);

        respawnManager.BreakRespawn();

        isWaweEnded = true;
        isTimeForNextWawe = true;
        taskIsSq = false;

        //StartCoroutine(BreathTimeBetweenTwoWawes());

        currentWaveIndex++;

    }

    private bool CheckToExecuteWave()
    {
        if (isWaweEnded == false)
            return false;

        if (isTimeForNextWawe == false)
            return false;

        if (taskIsSq)
            return false;

        if (wawesData == null || wawesData.Count == 0)
        {

            Debug.Log("Plz write Wawe Data first!");
            return false;
        }

        if (currentWaveIndex >= wawesData.Count)
        {
            startSaftyTriggerIn(5);

            if (respawnManager.isSceenClear || isSaftyAtEndTriggered)
            {
                GameManager.instance.LevelCompleted();
                isWaweEnded = true;
                isTimeForNextWawe = false;
                isSaftyAtEndTriggered = false;

            }

            return false;
        }

        return true;
    }

    public void StartWaves(LevelManager manager)
    {

        isStarted = true;

        isWaweEnded = true;
        isTimeForNextWawe = true;

        waweStartTime = Time.time;
        myManager = manager;
    }

    public void StopWaves()
    {
        ClearScene();
        respawnManager.BreakRespawn();
    }
    private void ResetWaweManager()
    {
        respawnManager.ClearScene();
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
    }

    public void OnEventEnded()
    {
        isWaweEnded = true;
        isTimeForNextWawe = true;
        taskIsSq = false;
    }

 

    private void startSaftyTriggerIn(float time)
    {
        Invoke(nameof(saftyCheck), time);
    }

    private void saftyCheck()
    {
        if (isSaftyAtEndTriggered == false)
            isSaftyAtEndTriggered = true;
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

        foreach (var bulletHolde in FindObjectsByType<BulletHoldeVfx>(FindObjectsSortMode.InstanceID))
            Destroy(bulletHolde.gameObject, .2f);
               

        StopAllCoroutines();

        respawnManager.ClearScene();
    }

}
