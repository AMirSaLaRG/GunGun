using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public GameObject levelItemPrefab;
    public Transform contentParent; // Assign the Content object
    public List<LevelItemData> levels = new List<LevelItemData>();

    private WaweManager currentWawes;

    public int currentLevelIndex {  get; private set; }
    
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

            int index = i; // Capture for closure
            btn.onClick.AddListener(() => OnLevelSelected(index));
        }
    }

    private void OnLevelSelected(int index)
    {
        Debug.Log($"Selected Level: {index + 1}");
        LoadLevel(index);
    }

    private void LoadLevel(int index)
    {
        LevelItemData levelToLoad = levels[index];

        foreach (var level in levels)
            foreach (var item in level.items)
                item.gameObject.SetActive(false);

        foreach (var item in levelToLoad.items)
            item.gameObject.SetActive(true);

        currentWawes = levelToLoad.waveManager;
        currentWawes.SetRespawnManager(levelToLoad.respawnManager);

        currentLevelIndex = index;

        GameManager.instance.OnLevelSelected();
    }
    public void LoadNextLevel()
    {
        currentLevelIndex++;

        int index = currentLevelIndex;
        LevelItemData levelToLoad = levels[index];

        foreach (var level in levels)
            foreach (var item in level.items)
                item.gameObject.SetActive(false);

        foreach (var item in levelToLoad.items)
            item.gameObject.SetActive(true);

        currentWawes = levelToLoad.waveManager;
        currentWawes.SetRespawnManager(levelToLoad.respawnManager);

        currentLevelIndex = index;

        GameManager.instance.OnLevelSelected();
    }


    public void SetGameStart(bool gameStart)
    {
        if (gameStart)
            currentWawes.StartWaves();

        else
            currentWawes.StopWaves();
    }

    public void ClearScene()
    {
        currentWawes.ClearScene();
    }

}
