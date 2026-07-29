using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class LeaderBoardContentUi : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI dateText;


    public void Setup(int rank, string name, float score, string levelName, string date)
    {
        rankText.text = rank.ToString();
        nameText.text = name;
        scoreText.text = score.ToString();
        levelNameText.text = levelName;
        dateText.text = date;

    }

    public void Header()
    {
        rankText.enableAutoSizing = false;
        rankText.fontSize = 36;

        rankText.text = "RANK";

        nameText.text = "NAME";
        scoreText.text = "SCORE";
        levelNameText.text = "LEVEL NAME";
        dateText.text = "DATE";
    }
}
