using UnityEngine;

[System.Serializable]
public class RewardData
{
    public ERewardType type;
    public bool hasAutoClaimed;
    public bool ClaimTime;
    public int amount;
}

[System.Serializable]
public class GunRewardOnComboData
{
    public int combo;
    public int gunId;
}

public enum ERewardType { Gun, Soldirecoin, Points}