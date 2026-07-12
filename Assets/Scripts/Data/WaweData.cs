using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class WaweData 
{
    public WaweType Type;

    public float durationIfTimeBase;
    public int respawnNumIfRespawnBase;
    public RespawnType respawnTypeIfRespawnBase;
    public float nextWaweDelay = 2f;
    public int ActiveBoxes;

    public Vector2Int minMaxxSummon = new Vector2Int(1,1);
    public List<RespawnData> respawns = new List<RespawnData>();

    public bool isWaweEnded = false;

}

public enum WaweType
{
    TimeBase = 0,
    RespawnBase = 1,
}



