using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string name;
    public float currentCoins;
    public int[] levelStars;      // stars per level
    public float[] levelPoints;     // points per level
    public int unlockedLevel = 1;
    public float musicVolume;
    public float sfxVolume;
    
}
