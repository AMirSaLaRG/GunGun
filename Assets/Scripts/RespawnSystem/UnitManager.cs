using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public List<RespawnData> basicRespawnUnitInfo = new List<RespawnData>();

    private List<Target> targetTracker = new List<Target>();

    public bool onWaitingRoomForSceneClear { private set; get; } = false;

    public void TrackTarget(Target target) => targetTracker.Add(target);

    public bool RemoveTrack(Target unit)
    {
        targetTracker.Remove(unit);
        bool isSceneClear = targetTracker.Count == 0;

        if (onWaitingRoomForSceneClear)
            onWaitingRoomForSceneClear = !isSceneClear;

        return isSceneClear;
    }

    public RespawnData GetBasicUnitData(RespawnType type)
    {
        RespawnData DataToReturn = basicRespawnUnitInfo.Find(x => x.respawnType == type);

        return DataToReturn;
    }


    public void SetOnWaitingForSceneClear()
    {
        if (targetTracker.Count == 0)
            return;

        onWaitingRoomForSceneClear = true;
    }

    internal void ClearTheScene()
    {
        foreach (var target in targetTracker)
        {
            if (target != null)
                Destroy(target.gameObject);
        }

        targetTracker.Clear();

        onWaitingRoomForSceneClear=false;
    }
}
