using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public List<RespawnData> basicRespawnUnitInfo = new List<RespawnData>();

    private List<Target> targetTracker = new List<Target>();

    public void TrackTarget(Target target) => targetTracker.Add(target);

    public bool RemoveTrack(Target unit)
    {
        targetTracker.Remove(unit);
        return (targetTracker.Count == 0);
    }

    public RespawnData GetBasicUnitData(RespawnType type)
    {
        return basicRespawnUnitInfo.Find(x => x.respawnType == type);

    }

}
