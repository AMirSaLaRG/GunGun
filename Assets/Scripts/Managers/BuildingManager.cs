using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    private LevelManager levelManager;

    [SerializeField] private List<Building> buildings = new List<Building>();
    [SerializeField] private List<Sprite> buildingsImage = new List<Sprite>();

    public EBuilding currentBuilding { private set; get; }
    public int currentBuildingStage { private set; get; }

    private List<EBuilding> availableBuildings = new List<EBuilding>();
    private int currentBuildingIndex = 0;

    private void Awake()
    {
        levelManager = FindFirstObjectByType<LevelManager>();
    }

    private void Start()
    {
        foreach (var building in buildings)
        {
            if (building.stage == 0)
                availableBuildings.Add(building.building);
        }

        LoadBuilding(availableBuildings[currentBuildingIndex], 0);
        SetLevelsForBuilding();

    }

    public void LoadBuilding(EBuilding building, int stage = 0)
    {
        foreach (var buildingItem in buildings)
        {
            foreach (var item in buildingItem.items)
                item.SetActive(false);

        }

        GameObject[] targetItems = buildings.Find(b => b.building == building && b.stage == stage)?.items;

        foreach (var item in targetItems)
            item.SetActive(true);

        currentBuilding = building;
        currentBuildingStage = stage;
    }

    [ContextMenu("Load PentHouse Stage 0")]
    public void LoadPentHouseStage0()
    {
        LoadBuilding(EBuilding.BasicTest, 0);
    }

    public void LoadNextBuilding()
    {
        if (!LoadDifBuilding(1))
        {
            Debug.Log("No more buildings to load.");
        }
    }

    public int GetBuildingsCount()
    {
        return availableBuildings.Count;
    }

    public void LoadPerviosBuilding()
    {
        if (!LoadDifBuilding(-1))
        {
            Debug.Log("No more buildings to load.");
        }
    }

    private bool LoadDifBuilding(int dif = 1)
    {
        int currentIndex = availableBuildings.IndexOf(currentBuilding);
        int targetIndex = currentIndex + dif;

        if (targetIndex < 0)
        {
            Debug.Log("No more buildings to load.");
            return false;
        }

        if (targetIndex < availableBuildings.Count)
        {
            Building targetData = buildings.Find(b => b.building == availableBuildings[targetIndex] && b.stage == 0);
            if (targetData.isLock)
            {
                Debug.Log("Target building is locked.");
                return false;
            }else
            {
                LoadBuilding(availableBuildings[targetIndex], 0);
                SetLevelsForBuilding();
                return true;
            }
                
        }
        else
        {
            Debug.Log("No more buildings to load.");
            return false;
        }


    }

    public void LoadBuildingStage(int buildingStage)
    {
        LoadBuilding(currentBuilding, buildingStage);
    }

    private void SetLevelsForBuilding()
    {
        Building targetData = buildings.Find(b => b.building == currentBuilding && b.stage == currentBuildingStage);
        LevelItemData[] levels = targetData.levels;

        levelManager.SetLevels(levels);
    }

    public Sprite GetBuildingSprite(EBuilding building)
    {
        int index = buildings.FindIndex(b =>b.building == building);
        if (buildingsImage.Count > index)
            return buildingsImage[index];

        return null;
    }
}
