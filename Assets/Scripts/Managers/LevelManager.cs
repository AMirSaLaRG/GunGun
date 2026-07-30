using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public RespawnManager respawnManager;

    public GameObject levelItemPrefab;
    public Transform contentParent; // Assign the Content object
    public List<LevelItemData> levels = new List<LevelItemData>();

    private WaweManager currentWaves;
    public float[] currentLevelStarPoints { private set; get; } = new float[3];
    public float threeStarPoint { private set; get; } = 1;

    public int currentLevelIndex {  get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        PopulateLevels();
        respawnManager = FindFirstObjectByType<RespawnManager>();
    }

    public void PopulateLevels()
    {
        // Clear existing items
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        PlayerData progressData = SaveManager.instance.data;
        // Create level items
        for (int i = 0; i < levels.Count; i++)
        {
            GameObject newItem = Instantiate(levelItemPrefab, contentParent);
            LevelItem itemScript = newItem.GetComponent<LevelItem>();

            

            if (itemScript != null)
            {
                itemScript.SetupLevel(levels[i], i);
            }

            // Add button for click functionality
            Button btn = newItem.GetComponent<Button>();
            if (btn == null)
                btn = newItem.AddComponent<Button>();

            if (i > progressData.unlockedLevel)
                btn.interactable = false;

            if (GameManager.instance.isTesting)
                btn.interactable = true;

            int index = i; // Capture for closure
            btn.onClick.AddListener(() => OnLevelSelected(index));
        }
    }
    public void Test(int index)
    {
        OnLevelSelected(index);

    }

    private void OnLevelSelected(int index)
    {
        Debug.Log($"Selected Level: {index + 1}");
        LoadLevel(index);
    }

    private void LoadLevel(int index)
    {
        LevelItemData levelToLoad = levels[index];

        respawnManager.SetupBoxes(levelToLoad.respawnBoxHolder);

        threeStarPoint = levelToLoad.pointsForthreeStar;

        foreach (var level in levels)
        {
            foreach (var item in level.items)
                item.gameObject.SetActive(false);

            level.respawnBoxHolder.gameObject.SetActive(false);
        }

        levels[index].respawnBoxHolder.gameObject.SetActive(true);

        foreach (var item in levelToLoad.items)
            item.gameObject.SetActive(true);

        SetUpLevelData(levelToLoad);

        currentLevelIndex = index;

        GameManager.instance.OnLevelSelected();
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;

        int index = currentLevelIndex;

        LoadLevel(index);
    }

    private void SetUpLevelData(LevelItemData levelToLoad)
    {
        currentWaves = levelToLoad.waveManager;
        currentWaves.SetRespawnManager(respawnManager);

        if (levelToLoad.pointsForOneStar == 0 || levelToLoad.pointsFortwoStar == 0 || levelToLoad.pointsForthreeStar == 0)
            Debug.LogWarning("Set The Star Points of Level");


        currentLevelStarPoints[0] = levelToLoad.pointsForOneStar;
        currentLevelStarPoints[1] = levelToLoad.pointsFortwoStar;
        currentLevelStarPoints[2] = levelToLoad.pointsForthreeStar;
    }

    public void SetGameStart(bool gameStart)
    {
        if (gameStart)
        {

            currentWaves.StartWaves();
        }

        else
        {
            currentWaves.StopWaves();
            currentWaves.ClearScene();
        }
    }

    public string GetLevelName(int index) => levels[index].levelName;



}
