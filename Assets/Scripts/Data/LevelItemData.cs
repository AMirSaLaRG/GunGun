using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class LevelItemData 
{
    public EBuilding Building;
    public int buildingStage = 0;
    public string levelName;
    public Sprite thumbnail;
    public WaweManager waveManager;
    public Transform respawnBoxHolder;
    public bool isLock = true;
    public float pointsForOneStar;
    public float pointsFortwoStar;
    public float pointsForthreeStar;

}

[System.Serializable]
public class PlayersLevelData
{
    public EBuilding Building;
    public int buildingStage = 0;
    public int levelIndex = 0;
    public int stars = 0;
    public float points = 0;
    public bool isLock = true;
}

public enum EBuilding
{
    None,
    PentHouse,
    StreetApartman,
    BasicTest,
}
