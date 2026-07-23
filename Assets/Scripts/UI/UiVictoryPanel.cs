using DG.Tweening;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UiVictoryPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointText;
    [SerializeField] private bool hasStar;
    [SerializeField] private Image[] StarImages;
    [SerializeField] private Color earnedStarColor = Color.yellow;

    private Vector3[] startStarPos;
    private Vector3[] startStarScale;

    private bool isConfigStartPos;

    PlayerController playerController;
    private UiCountTo pointCount;

    private void OnEnable()
    {
        
        if (pointCount == null)
            pointCount = pointText.AddComponent<UiCountTo>();

        if (playerController == null)
            playerController = GameManager.instance.playerController;

        pointCount.StartSettingTheNumber(playerController?.points?? 0, 1);

        if (hasStar == false)
            return;

        if (isConfigStartPos == false)
        {
            GetStartingPosAndScale();
        }

        int starEarned = GameManager.instance?.ChecklevelStarsEarned()?? 0;

        StarAnimation(starEarned);
    }

    private void GetStartingPosAndScale()
    {
        startStarPos = new Vector3[StarImages.Length];
        startStarScale = new Vector3[StarImages.Length];

        for (int i = 0; i < StarImages.Length; i++)
        {
            startStarPos[i] = StarImages[i].transform.localPosition;
            startStarScale[i] = StarImages[i].transform.localScale;
        }
    }

    private void OnDisable()
    {
        pointCount.ResetNumber();
    }

    private void StarAnimation(int earnedStars)
    {
        if (startStarScale == null)
            return;
        if (startStarPos == null)
            return;
        for (int i = 0; i < StarImages.Length; i++)
        {
         

            if (startStarPos[i] == null)
                return;
            if (startStarScale[i] == null)
                return;

            StarImages[i].transform.localScale = Vector3.zero;
            StarImages[i].transform.localPosition = new Vector3(startStarPos[i].x + Random.Range(-3000, 3000), startStarPos[i].y + Random.Range(-3000, 3000), startStarPos[i].z);

            if (i + 1 <= earnedStars)
                StarImages[i].color = earnedStarColor;

            StarImages[i].transform.DOLocalMove(startStarPos[i], 1f).SetEase(Ease.InOutElastic);
            StarImages[i].transform.DOScale(startStarScale[i], 1f);
        }
    }
}
