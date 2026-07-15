using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class WaveData 
{
    public WaveType Type;

    public LevelEventSequenceSo eventSequenceSo;

    public float durationIfTimeBase;
    public int respawnNumIfRespawnBase;
    public RespawnType respawnTypeIfRespawnBase;
    public float nextWaweDelay = 2f;
    public int ActiveBoxes;

    public Vector2Int minMaxNumSummon = new Vector2Int(1,1);
    public Vector2 minMaxRandomIntervalSummon = new Vector2(1,1);
    public List<RespawnData> respawns = new List<RespawnData>();

    public bool isWaweEnded = false;


}

public enum WaveType
{
    TimeBase = 0,
    RespawnBase = 1,
    EventBase = 3,
}



