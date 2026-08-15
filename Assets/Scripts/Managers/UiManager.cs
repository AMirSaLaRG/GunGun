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
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject credits;
    [SerializeField] private GameObject levelSelection;
    [SerializeField] private GameObject ready;
    [SerializeField] private GameObject inGame;
    [SerializeField] private GameObject pause;
    [SerializeField] private GameObject victory;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject LeaderBoard;

    [Header("MenuStatics")]
    [SerializeField] private GameObject onTopInMenu;

    [Header("DynamicPanels")]
    [SerializeField] private GameObject mainPageArmory;

    [Header("VisualHelper")]
    [SerializeField] private GameObject fadePanel;

    public EPanel currentPanel { private set; get; } = EPanel.MainMenu;


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
        LeaderBoard?.SetActive(panel == EPanel.LeaderBoard);

        onTopInMenu?.SetActive(panel != EPanel.InGame);


        currentPanel = panel;
    }

    public void PopDynamicPanel(EDPanel panel)
    {
        mainPageArmory?.SetActive(panel == EDPanel.Armory);
    }

    public void ExitArmory()
    {
        PopDynamicPanel(EDPanel.None);

    }
    public void OpenArmory()
    {
        PopDynamicPanel(EDPanel.Armory);
    }

    public GameObject GetPanel(EPanel panel)
    {
        switch (panel)
        {
            case EPanel.MainMenu: return mainMenu;
            case EPanel.Settings: return settings;
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

    public void FadePanel(float time)
    {
        SetPanel(EPanel.None);
        fadePanel.SetActive(true);
        fadePanel.GetComponent<Image>().DOFade(0, time).OnComplete(() => {
            fadePanel.SetActive(false);

        });
    }
}
