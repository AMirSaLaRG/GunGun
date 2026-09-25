using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillsUi : MonoBehaviour
{
    [Header("AdrenalineRush")]
    [SerializeField] private GameObject UiFxAr;
    [SerializeField] private TextMeshProUGUI aRTimer;
    [SerializeField] private Slider aRTimerSlider;
    [Header("GeneralWatch")]
    [SerializeField] private GameObject UiFxGw;
    [SerializeField] private TextMeshProUGUI gWTimer;
    [SerializeField] private Slider gWTimerSlider;
    [Header("GeneralWatch")]
    [SerializeField] private GameObject uiFxShield;




    private void Start()
    {
        UiFxAr.SetActive(false);
        UiFxGw.SetActive(false);
    }

    public void onAdrenalineRUsh(bool enable)
    {
        UiFxAr.SetActive(enable);
    }

    public void UpdateAdrenalineRushTime(float reminderTime, float maxTime)
    {
        aRTimer.text = reminderTime.ToString("0");
        aRTimerSlider.value = reminderTime / maxTime;
    }

    public void onActiveGeneralWatch(bool enable)
    {
        UiFxGw.SetActive(enable);

    }
    public void onActiveShield(bool enable)
    {
        uiFxShield.SetActive(enable);

    }

    public void UpdateGeneralWatchTime(float reminderTime, float aRDuration)
    {
        gWTimer.text = reminderTime.ToString("0");
        gWTimerSlider.value = reminderTime / aRDuration;
    }
}
