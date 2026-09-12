using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string name;
    public float totalPoints;
    public int soldirCoins;
    public PlayersLevelData[] playerLevelsData = new PlayersLevelData[0];
    public int[] unlockedWeapons;
    public float musicVolume;
    public float sfxVolume;
    public int lastGunId = 0;
}
