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


    [Header("OnCombo")]
    [SerializeField] private OnComboUi onComboElement;
    [SerializeField] private float onComboDuration;
    [SerializeField] private float onComboAndPointOffsetX;
    [SerializeField] private float onComboAndPointOffsetRandom;

    [Header("OnCashPointEarned")]
    [SerializeField] private OnCashPointUi onCashPointUi;
    [SerializeField] private float onPointDuration;
    [SerializeField] private float cashPointScale;




    [Header("TakingDamageUi")]
    [SerializeField] private Transform takingDamageElements;
    [SerializeField] private float takingDamageUiTime;

    private Image[] takingDamageImages;
    private OnHitUi[] onHitUis;
    private int prepareOnhitUis = 20;
    private int currentOnHitIndex = 0;
    private int currentKhalasIndex;

    private void Start()
    {
        takingDamageImages = takingDamageElements.GetComponentsInChildren<Image>();

        onHitUis = new OnHitUi[prepareOnhitUis];

        for (int i = 0; i < onHitUis.Length; i++)
        {
            onHitUis[i] = Instantiate(onHitUiPrefab, onHitUiHolder).GetComponent<OnHitUi>();
            onHitUis[i].transform.localScale = Vector3.zero;
        }
    }

    public void OnTakingDamage(Vector3 screenPoint)
    {
        takingDamageElements.position = new Vector3(screenPoint.x, screenPoint.y, takingDamageElements.position.z);

        screenPanel.color = Color.red;
        PulsFadeEffectAndFade(screenPanel, takingDamageUiTime);

        foreach (var image in takingDamageImages)
        {
            image.transform.localScale = Vector3.zero;
            image.transform.DOScale(1, takingDamageUiTime).SetEase(Ease.OutElastic);
            PulsFadeEffectAndFade(image, takingDamageUiTime);
        }
    }
    public void Onhit(Vector3 screenPoint, float distance, string text, Color color)
    {
        float scale = Mathf.Lerp(maxScale, minScale, distance / maxDistance);
        float randomX = UnityEngine.Random.Range(-onHitOffsetRandom, onHitOffsetRandom);
        float randomY = UnityEngine.Random.Range(onHitOffsetY - onHitOffsetRandom, onHitOffsetY + onHitOffsetRandom);

        Vector2 onHitOffset = new Vector2(randomX, randomY);

        onHitUis[currentOnHitIndex].Setup(screenPoint, text, color, scale, onHitOffset, onHitDuration);
        currentOnHitIndex = (currentOnHitIndex + 1) % onHitUis.Length; 
    }
    public void OnHitKhalas(Vector3 screenPoint, float distance, int combo, Color color)
    {
        if (combo == 1)
        {
            currentKhalasIndex = currentOnHitIndex;
            Onhit(screenPoint, distance, "KHALASS", color);
        } else
        {
            onHitUis[currentKhalasIndex].SetUpKhalas(combo);
        }
    }

    public void OnCombo(Vector3 screenPoint, float distance, int combo)
    {
        float scale = Mathf.Lerp(maxScale, minScale, distance / maxDistance);
        float randomX = UnityEngine.Random.Range(onComboAndPointOffsetX - onComboAndPointOffsetRandom, onComboAndPointOffsetX + onComboAndPointOffsetRandom);
        float randomY = UnityEngine.Random.Range(-onComboAndPointOffsetRandom, onComboAndPointOffsetRandom);

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
    private void PulsFadeEffectAndFade(Image targetImage, float duration)
    {

        targetImage.DOFade(.4f, duration / 2).OnComplete(() =>
        {
            targetImage.DOFade(0, duration / 2);
        });
    }

   
 
}
