using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class LevelItemData 
{
    public string levelName;
    public Sprite thumbnail;
    public int starsEarned;
    public int score;
    public WaweManager waveManager;
    public RespawnManager respawnManager;
    public Transform items;
    public bool isLock = true;
}
