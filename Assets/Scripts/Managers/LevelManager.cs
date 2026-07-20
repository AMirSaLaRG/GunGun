using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public GameObject levelItemPrefab;
    public Transform contentParent; // Assign the Content object
    public List<LevelItemData> levels = new List<LevelItemData>();

    private WaweManager currentWawes;

    private void Start()
    {
        PopulateLevels();
    }

    private void PopulateLevels()
    {
        // Clear existing items
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        GameManager.instance.PlayerData;
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

            if (levels[i].isLock == true && i != 0)
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
            level.items.gameObject.SetActive(false);

        levelToLoad.items.gameObject.SetActive(true);
        currentWawes = levelToLoad.waveManager;
        currentWawes.SetRespawnManager(levelToLoad.respawnManager);


        GameManager.instance.OnLevelSelected();
    }


    public void SetGameStart(bool gameStart)
    {
        if (gameStart)
        {
            currentWawes.isStarted = true;
        }
        else
        {
            currentWawes.isStarted = false;
            currentWawes.BreakOnWawe();

        }
    }
}
