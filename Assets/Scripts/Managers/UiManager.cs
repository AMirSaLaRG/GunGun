using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [Header("Pannels")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject credits;
    [SerializeField] private GameObject levelSelection;
    [SerializeField] private GameObject ready;
    [SerializeField] private GameObject inGame;
    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject victory;
    [SerializeField] private GameObject gameOver;

    [Header("In Game Elements")]
    [SerializeField] private TextMeshProUGUI gunAmoText;
    [SerializeField] private Button reloadBtn;
    [SerializeField] private GameObject hostageImageHolder;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI killText;
    [SerializeField] private TextMeshProUGUI PointsText;
    [SerializeField] private TextMeshProUGUI ComboRemindSecText;

    private EPanel currentPanel;

    private List<HostageImage> HostageImages = new List<HostageImage>();

    private float comboTimerReminder;

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

    public void SetPanel(EPanel panel)
    {
        mainMenu?.SetActive(panel == EPanel.MainMenu);
        settings?.SetActive(panel == EPanel.Settings);
        credits?.SetActive(panel == EPanel.Credits);
        ready?.SetActive(panel == EPanel.Ready);
        levelSelection?.SetActive(panel == EPanel.LevelSelection);
        inGame?.SetActive(panel == EPanel.InGame);
        pause?.SetActive(panel == EPanel.Pause);
        victory?.SetActive(panel == EPanel.Victory);
        gameOver?.SetActive(panel == EPanel.GameOver);

        currentPanel = panel;
    }

    public void UiOnHostageKill(int hostageKilled)
    {
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
        comboTimerReminder = Timer;

        InvokeRepeating(nameof(SetTimerText),0, .01f);
    }

    private void SetTimerText()
    {
        ComboRemindSecText.text = comboTimerReminder.ToString("0.00");
        comboTimerReminder -= .01f;

        if (comboTimerReminder < 0)
        {
            CancelInvoke(nameof (SetTimerText));
            comboTimerReminder = 0;
            ComboRemindSecText.text = comboTimerReminder.ToString("0.00");

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
        PointsText.text = points.ToString();
    }

    private void AssignButtons()
    {
        reloadBtn.onClick.AddListener(() =>
        {
            onReloadBtn?.Invoke();
        });
    }
}
