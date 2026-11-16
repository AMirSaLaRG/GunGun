using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int saveVersion;
    public string name;
    public float totalPoints;
    public int soldirCoins;

    public long rewardedAdNextAvailableUtcTicks;

    public PlayersLevelData[] playerLevelsData = new PlayersLevelData[0];

    public int[] unlockedWeapons;
    public float musicVolume;
    public float sfxVolume;
    public int lastGunId = 0;
    public bool completedTutorial = false;

    public string dailyTaskDate;
    public int dailyCombo;
    public int dailyKills;
    public bool dailyComboRewardClaimed;
    public bool dailyKillsRewardClaimed;
    public bool dailyVisitRewardClaimed;
}

