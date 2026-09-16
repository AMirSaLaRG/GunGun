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



    private void Start()
    {
        UiFxAr.SetActive(false);
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
}
