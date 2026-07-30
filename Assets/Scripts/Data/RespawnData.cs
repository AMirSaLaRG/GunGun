using UnityEngine;

[System.Serializable]
public class RespawnData
{
    public RespawnType respawnType;
    public GameObject prefab;
    public Sprite icon;
    [Range(0, 1)] public float respawnProb;


    public RespawnData Clone()
    {
        return new RespawnData
        {
            respawnType = this.respawnType,
            prefab = this.prefab,
            respawnProb = this.respawnProb
        };
    }
}

public enum RespawnType
{
    BaseEnemy = 0,
    Hostage = 1,
    TakerWithHostage = 2,
}



