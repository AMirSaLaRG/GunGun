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

    public Vector2Int minMaxxSummon = new Vector2Int(1,1);
    public List<RespawnData> respawns = new List<RespawnData>();

    public bool isWaweEnded = false;


    public WaveData GetShallowClone()
    {
        return new WaveData
        {
            Type = this.Type,
            eventSequenceSo = this.eventSequenceSo,
            durationIfTimeBase = this.durationIfTimeBase,
            respawnNumIfRespawnBase = this.respawnNumIfRespawnBase,
            respawnTypeIfRespawnBase = this.respawnTypeIfRespawnBase,
            nextWaweDelay = this.nextWaweDelay,
            ActiveBoxes = this.ActiveBoxes,
            minMaxxSummon = this.minMaxxSummon,
            respawns = new List<RespawnData>(this.respawns), // New list, same RespawnData objects
            isWaweEnded = this.isWaweEnded
        };
    }

    // Deep Clone: New list AND new RespawnData objects
    public WaveData GetDeepClone()
    {
        WaveData clone = new WaveData
        {
            Type = this.Type,
            eventSequenceSo = this.eventSequenceSo,
            durationIfTimeBase = this.durationIfTimeBase,
            respawnNumIfRespawnBase = this.respawnNumIfRespawnBase,
            respawnTypeIfRespawnBase = this.respawnTypeIfRespawnBase,
            nextWaweDelay = this.nextWaweDelay,
            ActiveBoxes = this.ActiveBoxes,
            minMaxxSummon = this.minMaxxSummon,
            isWaweEnded = this.isWaweEnded,
            respawns = new List<RespawnData>() // New empty list
        };

        // Deep copy each RespawnData
        foreach (var respawn in this.respawns)
        {
            if (respawn != null)
            {
                clone.respawns.Add(respawn.Clone()); // Clones each RespawnData
            }
        }

        return clone;
    }


}

public enum WaveType
{
    TimeBase = 0,
    RespawnBase = 1,
    EventBase = 3,
}



