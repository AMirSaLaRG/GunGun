using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UiVictoryPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointText;
    [SerializeField] private Image[] StarImages;

    private Vector3[] startStarPos;
    private Vector3[] startStarScale;

    PlayerController playerController;
    private UiCountTo pointCount;


    private void Start()
    {
        playerController = GameManager.instance.playerController;

        if (playerController == null)
            Debug.Log("Could Not FInd Player");


        pointCount = pointText.GetComponent<UiCountTo>();

        startStarPos= new Vector3[StarImages.Length];
        startStarScale = new Vector3[StarImages.Length];

        for (int i = 0; i < StarImages.Length; i++)
        {
            startStarPos[i] = StarImages[i].transform.localPosition;
            startStarScale[i] = StarImages[i].transform.localScale;
        }
  
    }

    private void OnEnable()
    {
        if (pointCount == null)
            pointCount = pointText.AddComponent<UiCountTo>();

        pointCount.StartSettingTheNumber(playerController?.points?? 0, 1);

        StarAnimation();
    }

    private void OnDisable()
    {
        pointCount.ResetNumber();
    }

    private void StarAnimation()
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

            StarImages[i].transform.DOLocalMove(startStarPos[i], 1f).SetEase(Ease.InOutElastic);
            StarImages[i].transform.DOScale(startStarScale[i], 1f);
        }
    }
}
