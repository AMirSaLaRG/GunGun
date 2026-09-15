using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Analytics.IAnalytic;
using static UnityEngine.GraphicsBuffer;

public class PlayerCanvas : MonoBehaviour
{
    private ScoreManager scoreManager;

    [SerializeField] private Image screenPanel;
    [Header("MainMessageText")]
    [SerializeField] private TextMeshProUGUI mainMsgText;
    private Coroutine mainTextCo;

    [Header("SecondaryWeapon")]
    [SerializeField] private Button switchWeaponBtn;
    [SerializeField] private Image gunIcon;

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
    [SerializeField] private string onEnemyText;
    [SerializeField] private Color onEnemyColor;
    [Space]
    [SerializeField] private string onShieldText;
    [SerializeField] private Color onShieldColor;
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
    [Space]
    [SerializeField] private string onProjectileHitText;
    [SerializeField] private Color onProjectileHitColor;


    [Header("OnCombo")]
    [SerializeField] private OnComboUi onComboElement;
    [SerializeField] private float onComboDuration;
    [SerializeField] private float onComboAndPointOffsetX;
    [SerializeField] private float onComboAndPointOffsetY;
    [SerializeField] private float onComboAndPointOffsetRandom;

    [Header("OnImunity")]
    [SerializeField] private Image ImunityImage;


    [Header("TakingDamageUi")]
    [SerializeField] private Transform takingDamageElements;
    [SerializeField] private float takingDamageUiTime;
    [Header ("TakeDamageAndDie")]
    [SerializeField] private Image DieVisualImage;
    [SerializeField] private float screenShakeIntansityDie = 0.3f;
    [SerializeField] private float screenShakeDurationDie = .3f;

    private OnHitUi[] onHitUis;
    private int prepareOnhitUis = 20;
    private int currentOnHitIndex = 0;
    private int currentKhalasIndex;

    private Camera mainCamera;
    private void Start()
    {
        scoreManager = FindFirstObjectByType<ScoreManager>();
        mainCamera = Camera.main;
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

        


        if (isDead)
        {
            if (GameManager.instance.isVibrateOn)
            {
                Debug.Log("Vibrate");
                Handheld.Vibrate();
            }

            StartCoroutine(DeadSpriteAnimation());
        }
    }

    private IEnumerator DeadSpriteAnimation()
    {
        

        yield return null;
        //screenPanel.color = Color.red;
        //screenPanel.DOFade(.4f, takingDamageUiTime / 2);
        DieVisualImage.transform.localEulerAngles = Vector3.forward * UnityEngine.Random.Range(0, 360);
        DieVisualImage.transform.localScale = Vector3.zero;
        DieVisualImage.transform.DOScale(1, takingDamageUiTime).SetEase(Ease.OutElastic).SetUpdate(true);
        DieVisualImage.DOFade(.4f, takingDamageUiTime / 2).SetUpdate(true) ;


      

        int saftyCheck = 0;
        while (true)
        {

            yield return new WaitForSeconds(.2f);
            if (GameManager.instance.gameState == EGameState.inMenu)
                break;
            saftyCheck++;
            if (saftyCheck > 6)
            {
                Debug.Log("Safty Check got triggered");
                break;
            }
        }

        DieVisualImage.DOFade(0f, takingDamageUiTime);
        screenPanel.DOFade(0f, takingDamageUiTime);



    }

    public void Onhit(Camera mainCaera, HitData hitData)
    {
        if (hitData == null) return;

        if (hitData == null) return;


        EHitType hitType = hitData.hitTYpe;

        if (hitType == EHitType.Magazine)
            return;

        if (hitType == EHitType.Khalas)
        {
            OnHitKhalas(mainCaera, hitData);
            return;
        }

        HandleOnHitVisual(mainCaera, hitData, hitType);

    
   


    }

    private void HandleOnHitVisual(Camera mainCaera, HitData hitData, EHitType hitType)
    {
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(hitData.hitPos);
        float distance = hitData.hitPos.z - mainCaera.transform.position.z;


        float scale = Mathf.Lerp(maxScale, minScale, distance / maxDistance);
        float randomY = UnityEngine.Random.Range(onHitOffsetY - onHitOffsetRandom, onHitOffsetY + onHitOffsetRandom);

        GetColorAndText(hitType, out Color color, out string text);

        Vector2 onHitOffset = new Vector2(0, randomY);

        onHitUis[currentOnHitIndex].Setup(screenPoint, text, color, scale, onHitOffset, onHitDuration);
        currentOnHitIndex = (currentOnHitIndex + 1) % onHitUis.Length;

        if (scoreManager.currentCombo > 1)
            OnCombo(screenPoint, distance);
  
    }

    public void OnHitKhalas(Camera mainCaera, HitData hitData)
    {
        int khalasCombo = scoreManager.khalasShotCount;

        if (khalasCombo == 1)
        {
            currentKhalasIndex = currentOnHitIndex;
            HandleOnHitVisual(mainCaera, hitData, EHitType.Khalas);
        }
        else
        {
            onHitUis[currentKhalasIndex].SetUpKhalas(khalasCombo);
        }
    }


    public void OnCombo(Vector3 screenPoint, float distance)
    {
        float scale = Mathf.Lerp(maxScale, minScale, distance / maxDistance);
        float randomX = UnityEngine.Random.Range(onComboAndPointOffsetX - onComboAndPointOffsetRandom, onComboAndPointOffsetX + onComboAndPointOffsetRandom);
        float randomY = UnityEngine.Random.Range(onComboAndPointOffsetY - onComboAndPointOffsetRandom, onComboAndPointOffsetY+ onComboAndPointOffsetRandom);

        Vector2 pointAndComboOffset = new Vector2(randomX, randomY);

        int combo = scoreManager.currentCombo;

        if (combo > 1)
        {
            onComboElement.Setup(screenPoint, combo, scale, pointAndComboOffset, onComboDuration);
        }
    }


    private void PulsFadeEffectAndFade(Image targetImage, float duration)
    {

        targetImage.DOFade(.4f, duration / 2).OnComplete(() =>
        {
            targetImage.DOFade(0, duration / 2);
        });
    }

    private void GetColorAndText(EHitType hitInfo, out Color color, out string text)
    {
        switch (hitInfo)
        {
            case EHitType.Enemy:
                color = onEnemyColor;
                text = onEnemyText;
                break;
            case EHitType.Shield:
                color = onShieldColor;
                text = onShieldText;
                break;
            case EHitType.MovingEnemy:
                color = onMovingEnemyColor;
                text = onMovingEnemyext;
                break;
            case EHitType.Missed:
                color = onMissColor;
                text = onMissText;
                break;
            case EHitType.Hostage:
                color = onHostageKillColor;
                text = onHostageKillText;
                break;
            case EHitType.Khalas:
                color = onKhalasColor;
                text = onKhalasText;
                break;
            case EHitType.EnemyProjectile:
                color = onProjectileHitColor;
                text = onProjectileHitText;
                break;

            default:
                color = UnityEngine.Color.white;
                text = "------";
                break;
        }
    }
    private void ShakeScreen(float duration, float intensity, Transform mainCamera)
    {
        StartCoroutine(ShakeScreenCo(duration, intensity, mainCamera));
    }

    public void ShakeScreen()
    {
        ShakeScreen(screenShakeDurationDie, screenShakeIntansityDie, mainCamera.transform);
    }

    private IEnumerator ShakeScreenCo(float duration, float intensity, Transform mainCamera)
    {
        Vector3 originalPos = Camera.main.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {

            float x = UnityEngine.Random.Range(-1f, 1f) * intensity;
            float y = UnityEngine.Random.Range(-1f, 1f) * intensity;
            mainCamera.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.localPosition = originalPos;
    }

    public void waveChange(WaveData data, string waveNumber)
    {
        if (mainTextCo != null)
        {
            StopCoroutine(mainTextCo);
            
        }

        if (data.waveText == null ||  data.waveText.Length == 0)
            mainMsgText.text = $"Get Ready For Wave:\n {waveNumber}";
        else
            mainMsgText.text = $"{data.waveText}: \n { waveNumber}";


        Color textC = mainMsgText.color;
        textC.a = 1;
        mainMsgText.color = textC;


        ClearMainText(data.startingDelay);
    }

    private void ClearMainText(float displayTime)
    {
        mainTextCo = StartCoroutine(ClearMainTextCo(displayTime));
    }

    private IEnumerator ClearMainTextCo(float displayTime = 1)
    {
        float elapse = 0;
        Color theColor = mainMsgText.color;
        Color orginalColor = mainMsgText.color;

        while (elapse < displayTime)
        {
            theColor.a = Mathf.Lerp(1, 0, elapse / displayTime);
            mainMsgText.color = theColor;
            yield return null;
            elapse += Time.deltaTime;
        }

        theColor.a = 0;
        mainMsgText.color = theColor;
    }

    public void OnImunity(float time)
    {
        float startingAlpha = 1;
        float endAlpha = 0;

        Debug.Log(time);


        ImunityImage.DOFade(startingAlpha, 0);
        ImunityImage.DOFade(endAlpha, time * 5/6);
    }

    internal void ShowSwitchBtn(bool enable, Sprite gunIcon)
    {
        switchWeaponBtn.gameObject.SetActive(enable);

        if (gunIcon == null)
            this.gunIcon.sprite = null;
        else
            this.gunIcon.sprite = gunIcon;
        
    }
}
