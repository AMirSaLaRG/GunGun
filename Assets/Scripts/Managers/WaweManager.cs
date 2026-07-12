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
    [SerializeField] private List<WaweData> waweData;
    public bool isStarted;

    private float timeLastWaweEnded = 0;
    private bool isWaweEnded = true;
    private bool isTimeForNextWawe = true;
    private int currentWaveIndex = 0;

    private void Awake()
    {
        respawnManager = FindFirstObjectByType<RespawnManager>();

        respawnManager.jobDone += RespawnManagerTaskOver;
    }

    private void Update()
    {
        if (isStarted == false)
            return;

        StartWawe();
    }


    private void StartWawe()
    {
        if (isWaweEnded == false)
            return;

        if (isTimeForNextWawe == false)
            return;

        if (waweData == null || waweData.Count == 0)
        {
            Debug.Log("Plz write Wawe Data first!");
            return;
        }

        if (currentWaveIndex == waweData.Count)
        {
            Debug.Log("all wawes executed!");
            isWaweEnded = true;
            isTimeForNextWawe = false;
            return;
        }



        ExecuteWawe(waweData[currentWaveIndex]);
    }

    private void ExecuteWawe(WaweData data)
    {
        CleanData(data);

        isWaweEnded = false;
        isTimeForNextWawe = false;
        currentWaveIndex++;
    

        respawnManager.SetupRandom(data.respawns, data.ActiveBoxes, data.minMaxxSummon, data.respawnNumIfRespawnBase, data.respawnTypeIfRespawnBase);

        if (data.Type == WaweType.TimeBase)
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
    private void CleanData(WaweData data)
    {
        CleanRepeat(data);
        CleanProbs(data);

    }

    private static void CleanRepeat(WaweData data)
    {
        var group = data.respawns.GroupBy(x => x.respawnType).Select(group => new RespawnData
        {
            respawnType = group.Key,
            respawnProb = group.Average(x => x.respawnProb),
            targetType = group.First().targetType,
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

        float duration = waweData[currentWaveIndex - 1].nextWaweDelay;

        yield return new WaitForSeconds(duration);
        isTimeForNextWawe = true;
    }

    private static void CleanProbs(WaweData data)
    {
        List<float> probs = new List<float>();

        foreach (RespawnData rData in data.respawns)
            probs.Add(rData.respawnProb);

        float sumProbs = probs.Sum();

        for (int i = 0; i < probs.Count; i++)
            data.respawns[i].respawnProb = data.respawns[i].respawnProb / sumProbs;
    }
}
