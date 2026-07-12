using UnityEngine;

[System.Serializable]
public class RespawnData
{
    public RespawnType respawnType;
    public TargetType targetType;
    public GameObject prefab;
    [Range(0, 1)] public float respawnProb;

}

public enum RespawnType
{
    Hostage = 0,
    BaseEnemy = 1,

}
public enum TargetType
{
    Enemy = 0,
    Hostage = 1,
    Object = 2,

}
