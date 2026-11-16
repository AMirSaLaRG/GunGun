using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardManager : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Transform content;
    [SerializeField] private GameObject contentPrefab;
    [SerializeField] private ScrollRect scrollRect;

    private LeaderBoardContentUi activatedContant;

    private void OnEnable()
    {
        PopulateContent();
    }

    private void PopulateContent()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        List<LeaderBoardData> data = SaveManager.instance.leaderBoardDataList;

        LeaderBoardContentUi header =
            Instantiate(contentPrefab, content).GetComponent<LeaderBoardContentUi>();

        header.Header();

        for (int i = 0; i < data.Count; i++)
        {
            LeaderBoardContentUi newContent =
                Instantiate(contentPrefab, content).GetComponent<LeaderBoardContentUi>();

        

            newContent.Setup(
                i + 1,
                data[i].name,
                data[i].points,
                data[i].levelName,
                data[i].date);


            if (GameManager.instance.currentLeaderBoardRank != 0)
                if (i + 1 == GameManager.instance.currentLeaderBoardRank)
                {
                    newContent.GetHighlighted();
                    activatedContant = newContent;
                }
        }

        StartCoroutine(FocusCurrentRank(data.Count));
    }

    private IEnumerator FocusCurrentRank(int entryCount)
    {
        // Wait until Unity updates the layout of the instantiated rows.
        yield return null;


        int currentRank = GameManager.instance.currentLeaderBoardRank;

    
        currentRank = Mathf.Clamp(currentRank, 1, entryCount);

        // The header is the first child, so the player row starts at index 1.
        float normalizedPosition =
            1f - (float)currentRank / entryCount;

        scrollRect.verticalNormalizedPosition = normalizedPosition;
    }
}
