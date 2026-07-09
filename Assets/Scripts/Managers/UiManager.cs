using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private TextMeshProUGUI gunAmoText;
    [SerializeField] private Button reloadBtn;
    [SerializeField] private GameObject hostageImageHolder;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI killText;
    [SerializeField] private TextMeshProUGUI PointsText;
    [SerializeField] private TextMeshProUGUI ComboRemindSecText;


    private List<HostageImage> HostageImages = new List<HostageImage>();

    private float comboTimerReminder;

    public Action onReloadBtn;
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
        for (int i = 0; i < hostageKilled; i++)
        {
            if (HostageImages.Count > i)
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
