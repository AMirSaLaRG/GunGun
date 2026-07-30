using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiInGame : MonoBehaviour
{
    [Header("In Game Elements")]
    [SerializeField] private TextMeshProUGUI gunAmoText;
    [SerializeField] private Button reloadBtn;
    [SerializeField] private GameObject hostageImageHolder;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private Slider comboSlider;

    [SerializeField] private TextMeshProUGUI killText;
    [SerializeField] private TextMeshProUGUI ComboRemindSecText;
    [Header("PointAndStarTracker")]
    [SerializeField] private TextMeshProUGUI PointsText;
    [SerializeField] private Slider pointsSlider;
    [SerializeField] private Transform oneStarIcon;
    [SerializeField] private Transform twoStarIcon;
    [SerializeField] private Transform threeStarIcon;
    [SerializeField] private float starScaleAnimationTime;

    private Coroutine pointSliderCo;

    private List<HostageImage> HostageImages = new List<HostageImage>();

    private float currentComboTimerRemind;
    public Action onReloadBtn;
    private Coroutine reloadWarningCo;

    private void Awake()
    {
        AssignButtons();
    }

    private void Start()
    {
        HostageImages = hostageImageHolder.GetComponentsInChildren<HostageImage>().ToList();
    }

    public void UiOnHostageKill(int hostageKilled)
    {
        foreach (var image in HostageImages)
            image.CrossHostage(false);

        for (int i = 0; i < hostageKilled; i++)
        {
            if (HostageImages.Count <= i)
                return;
            HostageImages[i].CrossHostage(true);
        }
    }

    public void ResetHostageKill()
    {
        foreach (var image in HostageImages)
        {
            image?.CrossHostage(false);
        }
    }

    public void WarningReloadBtn(bool enable)
    {
        if (gameObject.activeInHierarchy == false)
            return;
        if (reloadWarningCo != null)
            StopCoroutine(reloadWarningCo);
        reloadWarningCo = StartCoroutine(WarningCo(enable));
    }

    private IEnumerator WarningCo(bool enable)
    {
        Image btnImage = reloadBtn.GetComponent<Image>();
        Color btnImageColor = btnImage.color;

        int direction = 1;

        while (enable)
        {
            btnImageColor.a += .005f * direction;
            if (btnImageColor.a > .3f)
                direction = -1;
            else if (btnImageColor.a <= .01f)
                direction = 1;

            btnImage.color = btnImageColor;
            yield return null;
        }

        btnImageColor.a = 0;
        btnImage.color = btnImageColor;
    }

    public void SetComboTimer(float Timer)
    {
        CancelInvoke(nameof(SetTimerText));
        currentComboTimerRemind = Timer;
        SetComboSliderMaxValue(Timer);

        InvokeRepeating(nameof(SetTimerText), 0, .01f);
    }

    private void SetTimerText()
    {
        ComboRemindSecText.text = currentComboTimerRemind.ToString("0.00");
        currentComboTimerRemind -= .01f;

        HandleComboSlider();

        if (currentComboTimerRemind < 0)
        {
            CancelInvoke(nameof(SetTimerText));
            currentComboTimerRemind = 0;
            ComboRemindSecText.text = currentComboTimerRemind.ToString("0.00");

        }
    }

    public void AmoChange(int amo)
    {
        gunAmoText.text = amo.ToString();
    }

    public void ComboChange(int combo)
    {
        comboText.text = combo.ToString();
    }

    public void KillChange(int kills)
    {
        killText.text = kills.ToString();
    }

    public void PointChange(float points)
    {
        PointsText.text = points.ToString(".0");
        HandleSliderOnPointChange(points);
    }

    private void AssignButtons()
    {
        reloadBtn.onClick.AddListener(() =>
        {
            onReloadBtn?.Invoke();
        });
    }

    private void HandleSliderOnPointChange(float points)
    {
        if (GameManager.instance.gameState != EGameState.inGame)
            return;

        if (points == 0)
        {

            pointsSlider.maxValue = LevelManager.instance.threeStarPoint;
            oneStarIcon.localScale = Vector3.zero;
            twoStarIcon.localScale = Vector3.zero;
            threeStarIcon.localScale = Vector3.zero;

            pointsSlider.value = points;

        }


        if (pointSliderCo != null)
            StopCoroutine(pointSliderCo);
        pointSliderCo = StartCoroutine(slideAnimationCo(points, pointsSlider));

        if (points >= LevelManager.instance.currentLevelStarPoints[0] && oneStarIcon.localScale == Vector3.zero)
        {
            AnimateStarsInPop(oneStarIcon, 1);
        }
        if (points >= LevelManager.instance.currentLevelStarPoints[1] && twoStarIcon.localScale == Vector3.zero)
        {
            AnimateStarsInPop(twoStarIcon, 1);
        }
        if (points >= LevelManager.instance.currentLevelStarPoints[2] && threeStarIcon.localScale == Vector3.zero)
        {
            AnimateStarsInPop(threeStarIcon, 1);
        }
        
    }

    private void AnimateStarsInPop(Transform star, float scale)
    {
        star.localScale = Vector3.zero;
        star.DOScale(scale, starScaleAnimationTime)
       .SetEase(Ease.OutBack);

        star.DOLocalRotate(new Vector3(0, 0, 360 * 4), starScaleAnimationTime, RotateMode.LocalAxisAdd)
            .SetEase(Ease.OutQuad);
    }

    private IEnumerator slideAnimationCo(float point, Slider slider, float smooth = 50)
    {
        float currentPoint = slider.value;
        float currentState = 0;
        float currentValue = 0;

        while (currentState < smooth)
        {
            currentValue = Mathf.Lerp(currentPoint, point, currentState / smooth);
            slider.value = currentValue;
            currentState++;
            yield return null;

        }
    }

    private void SetComboSliderMaxValue(float maxTime)
    {
        if (comboSlider.maxValue != maxTime)
            comboSlider.maxValue = maxTime;

        comboSlider.value = maxTime;

    }

    private void HandleComboSlider()
    {
        comboSlider.value = currentComboTimerRemind;

    }

    private void OnDisable()
    {
        StopAllCoroutines();
        DOTween.Kill(gameObject);
    }
}
