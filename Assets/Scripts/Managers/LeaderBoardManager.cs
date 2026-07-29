using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoardManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform content;
    [SerializeField] private GameObject contentPrefab;

    private void Start()
    {
        PapulateContent();
    }

    private void PapulateContent()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        List<LeaderBoardData> data = SaveManager.instance.leaderBoardDataList;

        LeaderBoardContentUi header = Instantiate(contentPrefab, content).GetComponent<LeaderBoardContentUi>();

        header.Header();

        for (int i = 0; i < data.Count; i++)
        {
            LeaderBoardContentUi newContent = Instantiate(contentPrefab, content).GetComponent<LeaderBoardContentUi>();

            newContent.Setup(i + 1, data[i].name, data[i].points, data[i].levelName.ToString(), data[i].date);
        }
    }

    private void OnEnable()
    {
        PapulateContent();
    }
}
