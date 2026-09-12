using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class LevelItem : MonoBehaviour
{
    public Image thumbnailImage;
    public TextMeshProUGUI levelNameText;
    public Image[] starImages; // 3 stars
    public TextMeshProUGUI scoreText;

    private int levelIndex;

    public void SetupLevel(LevelItemData data, int index)
    {
        levelIndex = index;
        levelNameText.text = data.levelName;
        thumbnailImage.sprite = data.thumbnail;

        // Fetch score using Building, Stage, and Level Index
        float currentPoints = SaveManager.instance.GetPoints(data.Building, data.buildingStage, index);
        scoreText.text = $"{currentPoints}";

        // Fetch stars using Building, Stage, and Level Index
        int earnedStars = SaveManager.instance.GetStars(data.Building, data.buildingStage, index);
        SetStars(earnedStars);

        // Optional: Lock / Unlock visual state check
        PlayersLevelData saveProgress = SaveManager.instance.GetLevelData(data.Building, data.buildingStage, index);

        // If no save data exists yet, default to the preset isLock value in LevelItemData
        bool isLocked = saveProgress != null ? saveProgress.isLock : data.isLock;

        // Update button interactable or overlay depending on lock status
        if (GetComponent<UnityEngine.UI.Button>() != null)
        {
            GetComponent<UnityEngine.UI.Button>().interactable = !isLocked;
        }
    }

    void SetStars(int earned)
    {
        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].color = i < earned ? Color.yellow : Color.gray;
        }
    }

    public void OnLevelClicked()
    {
        // Load the level
        Debug.Log($"Loading Level {levelIndex}");
        // SceneManager.LoadScene(levelIndex + 1);
    }
}
