using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaweManager : MonoBehaviour
{
    private RespawnManager respawnManager;
    [Header ("Setup")]
    [SerializeField] private List<WaweData> waweData;

    private float timeLastWaweEnded = 0;
    private bool isWaweEnded = true;
    private void Awake()
    {
        respawnManager = FindFirstObjectByType<RespawnManager>();
    }

    private void StartWawe()
    {
        if (waweData == null || waweData.Count == 0)
        {
            Debug.Log("Plz write Wawe Data first!");
            return;
        }

        foreach (WaweData data in waweData)
            ExecuteWawe(data);
    }

    private void ExecuteWawe(WaweData data)
    {
        isWaweEnded = false;
        CleanData(data);

        //respawnManager.Setup(data);

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
