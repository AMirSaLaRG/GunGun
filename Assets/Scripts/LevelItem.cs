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
        scoreText.text = $"Score: {SaveManager.instance.GetPoints(index)}";
        SetStars(SaveManager.instance.GetStarts(index));
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
