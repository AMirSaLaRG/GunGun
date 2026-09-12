using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public RespawnManager respawnManager;
    public BuildingManager buildingManager;
    public UiManager uiManager;

    public GameObject levelItemPrefab;
    public Transform contentParent; // Assign the Content object
    public List<LevelItemData> levels = new List<LevelItemData>();
    public LevelItem perviewData;


    private WaweManager currentWaves;
    public float[] currentLevelStarPoints { private set; get; } = new float[3];
    public float threeStarPoint { private set; get; } = 1;

    public int currentLevelIndex {  get; private set; }

    [Header("Sfx")]
    [SerializeField] private AudioSource levelStartSfx;
    [SerializeField] private AudioSource waveChangeSfx;
    [SerializeField] private AudioSource finalWaveSfx;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        respawnManager = FindFirstObjectByType<RespawnManager>();
        buildingManager = FindFirstObjectByType<BuildingManager>();
        uiManager = FindFirstObjectByType<UiManager>();
    }

    private void Start()
    {
        PopulateLevels();

    }

    public void PopulateLevels()
    {
        // Clear existing items
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        if (SaveManager.instance == null)
            return;

        // Filter levels for the currently active building and stage
        EBuilding currentBuilding = buildingManager.currentBuilding;
        int currentStage = buildingManager.currentBuildingStage;

        // Filter levels matching the current building and stage setup
        List<LevelItemData> activeStageLevels = levels.FindAll(l =>
            l.Building == currentBuilding && l.buildingStage == currentStage);

        // Create level items
        for (int i = 0; i < activeStageLevels.Count; i++)
        {
            LevelItemData levelData = activeStageLevels[i];
            GameObject newItem = Instantiate(levelItemPrefab, contentParent);
            LevelItem itemScript = newItem.GetComponent<LevelItem>();

            if (itemScript != null)
            {
                itemScript.SetupLevel(levelData, i);
            }

            // Get or add button for click listeners
            Button btn = newItem.GetComponent<Button>();
            if (btn == null)
                btn = newItem.AddComponent<Button>();

            // Check lock status from SaveManager (Level 0 defaults to unlocked)
            PlayersLevelData saveProgress = SaveManager.instance.GetLevelData(levelData.Building, levelData.buildingStage, i);

            bool isLocked = saveProgress != null ? saveProgress.isLock : (i > 0);

            // Lock button unless in testing mode
            btn.interactable = !isLocked;

            if (GameManager.instance != null && GameManager.instance.isTesting)
                btn.interactable = true;

            int levelIndex = i; // Capture loop variable for closure
            btn.onClick.AddListener(() => OnLevelSelected(levelIndex));
        }
    }
    public void Test(int index)
    {
        OnLevelSelected(index);

    }

    private void OnLevelSelected(int index)
    {
        LoadLevel(index);

    }

    private void LoadLevel(int index)
    {
        LevelItemData levelToLoad = levels[index];

        uiManager.ExitDynamicUi();
        perviewData.SetupLevel(levels[index], index);

        respawnManager.SetupBoxes(levelToLoad.respawnBoxHolder);

        threeStarPoint = levelToLoad.pointsForthreeStar;

        foreach(var level in levels)
        {
            level.respawnBoxHolder.gameObject.SetActive(false);
        }

        buildingManager.LoadBuildingStage(levels[index].buildingStage);

        levels[index].respawnBoxHolder.gameObject.SetActive(true);

        SetUpLevelData(levelToLoad);

        currentLevelIndex = index;

        GameManager.instance.OnLevelSelected();

        
    }

    public bool LoadNextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex >= levels.Count)
        {
            Debug.Log("No more levels to load.");
            return false;
        }

        int index = currentLevelIndex;

        LoadLevel(index);
        return true;
    }

    private void SetUpLevelData(LevelItemData levelToLoad)
    {
        currentWaves = levelToLoad.waveManager;

        if (levelToLoad.pointsForOneStar == 0 || levelToLoad.pointsFortwoStar == 0 || levelToLoad.pointsForthreeStar == 0)
            Debug.LogWarning("Set The Star Points of Level");


        currentLevelStarPoints[0] = levelToLoad.pointsForOneStar;
        currentLevelStarPoints[1] = levelToLoad.pointsFortwoStar;
        currentLevelStarPoints[2] = levelToLoad.pointsForthreeStar;
    }

    public void SetGameStart()
    {
        respawnManager.SetupBoxes(levels[currentLevelIndex].respawnBoxHolder);

        currentWaves.StartWaves(this);
    }
    
    public void SetGameEnd()
    {
        currentWaves.StopWaves();
        currentWaves.ClearScene();
    }
    public string GetLevelName(int index) => levels[index].levelName;

    public AudioSource GetLevelStartSfx() => levelStartSfx;
    public AudioSource GetWaveChangeSfx() => waveChangeSfx;
    public AudioSource GetFinalWaveSfx() => finalWaveSfx;

    internal void SetLevels(LevelItemData[] levels)
    {
        this.levels.Clear();
        this.levels.AddRange(levels);
    }

    public bool IsAllLevelsCompeleted()
    {
        int lastLevelIndex = levels.Count - 1;
        int start = SaveManager.instance.GetStars(buildingManager.currentBuilding, buildingManager.currentBuildingStage, lastLevelIndex);


        return start > 0;
    }
}
