using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    private LevelManager levelManager;

    [Header("LoadingAimationSetup")]
    [SerializeField] private float animationOffsetY = 50f;
    [SerializeField] private float animationTIme = 1f;
    [SerializeField] private AudioSource animSound;

    [SerializeField] private TextMeshProUGUI nameDisplay;
    [SerializeField] private Button nextBtn;
    [SerializeField] private Button pervBtn;
    [SerializeField] private Button pickBtn;

    [SerializeField] private List<Building> buildings = new List<Building>();
    [SerializeField] private List<Sprite> buildingsImage = new List<Sprite>();


    public EBuilding currentBuilding { private set; get; }
    public int currentBuildingStage { private set; get; }

    private List<EBuilding> availableBuildings = new List<EBuilding>();
    private int currentBuildingIndex = 0;

    Vector3 startingPos;

    private void Awake()
    {
        levelManager = FindFirstObjectByType<LevelManager>();

        foreach (var b in buildings)
            if (b != null)
                b.gameObject.SetActive(true);

        startingPos = transform.position;

    }

    private void Start()
    {
        foreach (var building in buildings)
        {
            if (building == null) continue;
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
        currentBuildingIndex = availableBuildings.IndexOf(currentBuilding);

        nameDisplay.text = currentBuilding.ToString();

        LoadAnimation();

        CheckForLastOrFirstLevel();
    }

    private void CheckForLastOrFirstLevel()
    {
        nextBtn.gameObject.SetActive(true);
        pervBtn.gameObject.SetActive(true);

        if (IsThisLastLevel())
        {
            nextBtn.gameObject.SetActive(false);
        } else if (IsThisFirstLevel())
        {
            pervBtn.gameObject.SetActive(false);
        } 
    }

    private bool IsThisFirstLevel()
    {
        return currentBuildingIndex == 0;
    }

    private bool IsThisLastLevel()
    {
        return currentBuildingIndex == buildings.Count - 1;
    }

    private void LoadAnimation()
    {
        transform.DOKill();


        Vector3 animPos = transform.position + new Vector3(0, animationOffsetY, 0);

        transform.position = animPos;

        transform.DOMove(startingPos, animationTIme).SetEase(Ease.OutBack);

        AudioManager.instance.PlaySfx(animSound, true);
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
            Building building = buildings.Find(b => b.building == availableBuildings[targetIndex] && b.stage == 0);
            Building targetData = building;
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
        int survivalDif = targetData.survivalDifficulty;

        levelManager.SetLevels(levels, survivalDif);
    }

    public Sprite GetBuildingSprite(EBuilding building)
    {
        int index = buildings.FindIndex(b =>b.building == building);
        if (buildingsImage.Count > index)
            return buildingsImage[index];

        return null;
    }

    public PowerUpWayPointsSet GetCurrentWayPointSet()
    {
        Building targetData = buildings.Find(b => b.building == currentBuilding && b.stage == currentBuildingStage);
        if (targetData != null && targetData.powerUpWayPoints != null)
            return targetData.powerUpWayPoints;
        return null;
    }

    public Building GetCurrentBuilding()
    {
        Building building = buildings.Find(b => b.building == availableBuildings[currentBuildingIndex] && b.stage == 0);

        return building;
            
    }
}
