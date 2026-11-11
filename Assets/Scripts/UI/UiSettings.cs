using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UiSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float mixerMultiplier = 25;

    [Header("SFX Settings")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private string sfxParamter = "SfxValume";
    [SerializeField] private TextMeshProUGUI sfxSliderText;

    [Header("BGM Settings")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private string bgmParamter = "BgmValume";
    [SerializeField] private TextMeshProUGUI bgmSliderText;

    private void Awake()
    {
    }

    private void Start()
    {
        Debug.Log("Run");

        StartingSetup();
    }

    public void StartingSetup()
    {

        float sfxV = PlayerPrefs.GetFloat(sfxParamter, .6f);
        float bgmV = PlayerPrefs.GetFloat(bgmParamter, .6f);

        float newValueSfx = MathF.Log10(sfxV) * mixerMultiplier;
        float newValueBgm = MathF.Log10(bgmV) * mixerMultiplier;

        audioMixer.SetFloat(sfxParamter, newValueSfx);
        audioMixer.SetFloat(bgmParamter, newValueBgm);
    }

    public void SFXSliderValue(float value)
    {
        float newValue = MathF.Log10(value) * mixerMultiplier;
        audioMixer.SetFloat(sfxParamter, newValue);

        sfxSliderText.text = Mathf.RoundToInt(value * 100) + "%";
    }

    public void BGMSliderValue(float value)
    {
        float newValue = MathF.Log10(value) * mixerMultiplier;
        audioMixer.SetFloat(bgmParamter, newValue);

        bgmSliderText.text = Mathf.Round(value * 100) + "%";
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(sfxParamter, sfxSlider.value);
        PlayerPrefs.SetFloat(bgmParamter, bgmSlider.value);
    }

    private void OnEnable()
    {

        sfxSlider.value = PlayerPrefs.GetFloat(sfxParamter, .6f);
        bgmSlider.value = PlayerPrefs.GetFloat(bgmParamter, .6f);
    }
}
