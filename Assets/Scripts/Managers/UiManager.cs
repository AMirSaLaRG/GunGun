using DG.Tweening;
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

    [Header("Panels")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject credits;
    [SerializeField] private GameObject levelSelection;
    [SerializeField] private GameObject ready;
    [SerializeField] private GameObject inGame;
    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject victory;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject lastChance;
    [SerializeField] private GameObject LeaderBoard;


    [Header("MenuDynamicOnTop")]
    [SerializeField] private GameObject onTopInMenu;
    [SerializeField] private GameObject onTopInfoInMenu;
    [SerializeField] private TextMeshProUGUI playersPoints;
    [SerializeField] private TextMeshProUGUI playersGems;


    [Header("DynamicPanels")]
    [SerializeField] private GameObject mainPageArmory;
    [SerializeField] private GameObject LeaderBoardD;
    [SerializeField] private GameObject levels;
    [SerializeField] private GameObject quitConfirm;
    [SerializeField] private GameObject settingsPopup;
    [SerializeField] private GameObject achievements;
    [SerializeField] private GameObject tasks;
    [SerializeField] private GameObject ads;
    [SerializeField] private GameObject dailyReward;

    [Header("VisualHelper")]
    [SerializeField] private GameObject fadePanel;

    public EPanel currentPanel { private set; get; } = EPanel.MainMenu;

    private void Start()
    {
        settingsPopup.GetComponent<UiSettings>().StartingSetup();
    }

    public void SetPanel(EPanel panel)
    {
        mainMenu?.SetActive(panel == EPanel.MainMenu);
        credits?.SetActive(panel == EPanel.Credits);
        ready?.SetActive(panel == EPanel.Ready);
        levelSelection?.SetActive(panel == EPanel.LevelSelection);
        inGame?.SetActive(panel == EPanel.InGame);
        pause?.SetActive(panel == EPanel.Pause);
        victory?.SetActive(panel == EPanel.Victory);
        gameOver?.SetActive(panel == EPanel.GameOver);
        LeaderBoard?.SetActive(panel == EPanel.LeaderBoard);
        lastChance?.SetActive(panel == EPanel.LastChance);

        onTopInMenu?.SetActive(panel != EPanel.InGame);
        onTopInfoInMenu?.SetActive(panel != EPanel.InGame);


        currentPanel = panel;

        UpdatePoints();
    }

    public void PopDynamicPanel(EDPanel panel)
    {
        mainPageArmory?.SetActive(panel == EDPanel.Armory);
        LeaderBoardD?.SetActive(panel == EDPanel.LeaderBoard);
        levels?.SetActive(panel == EDPanel.Levels);
        quitConfirm?.SetActive(panel == EDPanel.Quit);
        settingsPopup?.SetActive(panel == EDPanel.Settings);
        achievements?.SetActive(panel == EDPanel.Achievements);
        tasks?.SetActive(panel == EDPanel.Tasks);
        ads?.SetActive(panel == EDPanel.Ads);
        dailyReward?.SetActive(panel == EDPanel.DailyReward);

    }

    public void ExitDynamicUi()
    {
        PopDynamicPanel(EDPanel.None);

    }
    public void OpenArmory()
    {
        PopDynamicPanel(EDPanel.Armory);
    }
    public void OpenLeaderBoard()
    {
        PopDynamicPanel(EDPanel.LeaderBoard);
    }
    public void OpenLevels()
    {
        PopDynamicPanel(EDPanel.Levels);
    }
    public void OpenSettings()
    {
        PopDynamicPanel(EDPanel.Settings);
    }
    public void OpenAchievements()
    {
        PopDynamicPanel(EDPanel.Achievements);
    }
    public void OpenTasks()
    {
        PopDynamicPanel(EDPanel.Tasks);
    }
    public void OpenAds()
    {
        PopDynamicPanel(EDPanel.Ads);
    }
    public void OpenDailyReward()
    {
        PopDynamicPanel(EDPanel.DailyReward);
    }
    public void OpenQuitConfirm()
    {
        PopDynamicPanel(EDPanel.Quit);
    }


    public GameObject GetPanel(EPanel panel)
    {
        switch (panel)
        {
            case EPanel.MainMenu: return mainMenu;
            //case EPanel.Settings: return settings;
            case EPanel.Credits: return credits;
            case EPanel.Ready: return ready;
            case EPanel.LevelSelection: return levelSelection;
            case EPanel.InGame: return inGame;
            case EPanel.Pause: return pause;
            case EPanel.Victory: return victory;
            case EPanel.GameOver: return gameOver;
            case EPanel.LeaderBoard: return victory;
            default: return null;

        }
    }

    public void FadePanel(float time = .5f)
    {
        if (fadePanel == null) return;
        SetPanel(EPanel.None);
        Image img = fadePanel.GetComponent<Image>();

        Color c = img.color;
        c.a = 1;
        img.color = c;

        fadePanel.SetActive(true);
        img.DOFade(0, time).SetEase(Ease.InQuint).OnComplete(() => {
            fadePanel.SetActive(false);

        });
    }

    public void UpdatePoints()
    {
        if (GameManager.instance == null) return;
        int currentPoints = 0;
        int currentGems = 0;

        if (GameManager.instance.playerData != null)
            currentPoints = Mathf.FloorToInt(GameManager.instance.playerData.totalPoints);
        if (GameManager.instance.playerData != null)
            currentGems = Mathf.FloorToInt(GameManager.instance.playerData.soldirCoins);

        playersPoints.text = $"{currentPoints}";
        playersGems.text = $"{currentGems}";
    }
}
