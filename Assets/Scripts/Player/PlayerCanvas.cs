using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class PlayerCanvas : MonoBehaviour
{
    [SerializeField] private Image screenPanel;
    [Header("ScaleSetup")]
    [SerializeField] private float maxDistance;
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;

    [Header("OnHitUi")]
    [SerializeField] private GameObject onHitUiPrefab;
    [SerializeField] private Transform onHitUiHolder;
    [SerializeField] private float onHitDuration;
    [SerializeField] private float onHitOffsetY;
    [SerializeField] private float onHitOffsetRandom;
    [Space]
    [SerializeField] private string onEnemyext;
    [SerializeField] private Color onEnemyColor;
    [Space]
    [SerializeField] private string onMovingEnemyext;
    [SerializeField] private Color onMovingEnemyColor;
    [Space]
    [SerializeField] private string onMissText;
    [SerializeField] private Color onMissColor;
    [Space]
    [SerializeField] private string onHostageKillText;
    [SerializeField] private Color onHostageKillColor;
    [Space]
    [SerializeField] private string onKhalasText;
    [SerializeField] private Color onKhalasColor;


    [Header("OnCombo")]
    [SerializeField] private OnComboUi onComboElement;
    [SerializeField] private float onComboDuration;
    [SerializeField] private float onComboAndPointOffsetX;
    [SerializeField] private float onComboAndPointOffsetY;
    [SerializeField] private float onComboAndPointOffsetRandom;

    [Header("OnCashPointEarned")]
    [SerializeField] private OnCashPointUi onCashPointUi;
    [SerializeField] private float onPointDuration;
    [SerializeField] private float cashPointScale;

    [Header("LowAmoWarnning")]
    [SerializeField] private Transform lowAmoWarnningElements;
    [SerializeField] private Image lowAmoIcon;
    [SerializeField] private TextMeshProUGUI lowAmoText;
    [SerializeField] private float lowAmoScale = 2;
    [SerializeField] private float lowAmoDuration = 1;
    private bool isWarned = false;


    [Header("TakingDamageUi")]
    [SerializeField] private Transform takingDamageElements;
    [SerializeField] private Image DieVisualImage;
    [SerializeField] private float takingDamageUiTime;

    private OnHitUi[] onHitUis;
    private int prepareOnhitUis = 20;
    private int currentOnHitIndex = 0;
    private int currentKhalasIndex;

    private void Start()
    {

        onHitUis = new OnHitUi[prepareOnhitUis];

        for (int i = 0; i < onHitUis.Length; i++)
        {
            onHitUis[i] = Instantiate(onHitUiPrefab, onHitUiHolder).GetComponent<OnHitUi>();
            onHitUis[i].transform.localScale = Vector3.zero;
        }
    }

    public void OnTakingDamage(Vector3 screenPoint, bool isDead = true)
    {
        takingDamageElements.position = new Vector3(screenPoint.x, screenPoint.y, takingDamageElements.position.z);

        screenPanel.color = Color.red;
        PulsFadeEffectAndFade(screenPanel, takingDamageUiTime);

        if (isDead)
        {
            DieVisualImage.transform.localScale = Vector3.zero;
            DieVisualImage.transform.DOScale(1, takingDamageUiTime).SetEase(Ease.OutElastic);
            PulsFadeEffectAndFade(DieVisualImage, takingDamageUiTime);
        }

    }
    public void Onhit(Vector3 screenPoint, float distance, EHit hitInfo)
    {
        float scale = Mathf.Lerp(maxScale, minScale, distance / maxDistance);
        float randomY = UnityEngine.Random.Range(onHitOffsetY - onHitOffsetRandom, onHitOffsetY + onHitOffsetRandom);

        GetColorAndText(hitInfo, out Color color, out string text);

        Vector2 onHitOffset = new Vector2(0, randomY);

        onHitUis[currentOnHitIndex].Setup(screenPoint, text, color, scale, onHitOffset, onHitDuration);
        currentOnHitIndex = (currentOnHitIndex + 1) % onHitUis.Length; 
    }

 

    public void OnHitKhalas(Vector3 screenPoint, float distance, int combo, Color color)
    {
        if (combo == 1)
        {
            currentKhalasIndex = currentOnHitIndex;
            Onhit(screenPoint, distance, EHit.Khalas);
        } else
        {
            onHitUis[currentKhalasIndex].SetUpKhalas(combo);
        }
    }


    public void OnCombo(Vector3 screenPoint, float distance, int combo)
    {
        float scale = Mathf.Lerp(maxScale, minScale, distance / maxDistance);
        float randomX = UnityEngine.Random.Range(onComboAndPointOffsetX - onComboAndPointOffsetRandom, onComboAndPointOffsetX + onComboAndPointOffsetRandom);
        float randomY = UnityEngine.Random.Range(onComboAndPointOffsetY - onComboAndPointOffsetRandom, onComboAndPointOffsetY+ onComboAndPointOffsetRandom);

        Vector2 pointAndComboOffset = new Vector2(randomX, randomY);


        if (combo > 1)
        {
            onComboElement.Setup(screenPoint, combo, scale, pointAndComboOffset, onComboDuration);
        }
    }

    public void OnCashPoint(Vector3 screenPoint, float distance, float points)
    {

        int showPoints = Mathf.FloorToInt(points);

        onCashPointUi.SetUp(showPoints, cashPointScale, onPointDuration);
  
    }

    public void OnLowAmo()
    {
        if (isWarned)
            return;

        isWarned = true;

        DOTween.Kill(lowAmoWarnningElements.gameObject);

        lowAmoText.DOFade(1, 0);
        lowAmoIcon.DOFade(1, 0);

        PulsScaleEffect(lowAmoWarnningElements, lowAmoScale, lowAmoDuration, lowAmoText, lowAmoIcon);
    }

    public void OnReload()
    {
        isWarned = false;
        DOTween.Kill(lowAmoWarnningElements.gameObject);

        lowAmoWarnningElements.DOScale(0, 0);
    }
    private void PulsFadeEffectAndFade(Image targetImage, float duration)
    {

        targetImage.DOFade(.4f, duration / 2).OnComplete(() =>
        {
            targetImage.DOFade(0, duration / 2);
        });
    }
    private void PulsScaleEffect(Transform transform, float scale, float duration, TextMeshProUGUI text, Image icon)
    {
        transform.localScale = Vector3.zero;

        if (text)
            text.DOFade(0, duration);
        if (icon)
            icon.DOFade(0, duration);

        transform.DOScale(scale, duration * .7f).OnComplete(() =>
        {
            transform.DOScale(0, duration * .3f);
        });
    }
    private void GetColorAndText(EHit hitInfo, out Color color, out string text)
    {
        switch (hitInfo)
        {
            case EHit.Enemy:
                color = onEnemyColor;
                text = onEnemyext;
                break;
            case EHit.MovingEnemy:
                color = onMovingEnemyColor;
                text = onMovingEnemyext;
                break;
            case EHit.Missed:
                color = onMissColor;
                text = onMissText;
                break;
            case EHit.Hostage:
                color = onHostageKillColor;
                text = onHostageKillText;
                break;
            case EHit.Khalas:
                color = onKhalasColor;
                text = onKhalasText;
                break;

            default:
                color = UnityEngine.Color.white;
                text = "------";
                break;
        }
    }

}
