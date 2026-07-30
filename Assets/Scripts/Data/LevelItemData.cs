using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class LevelItemData 
{
    public string levelName;
    public Sprite thumbnail;
    public WaweManager waveManager;
    public Transform respawnBoxHolder;
    public Transform[] items;
    public bool isLock = true;
    public float pointsForOneStar;
    public float pointsFortwoStar;
    public float pointsForthreeStar;

}
