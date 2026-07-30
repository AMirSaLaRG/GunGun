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
    [SerializeField] private GameObject LeaderBoard;

    [SerializeField] private GameObject fadePanel;

    private EPanel currentPanel;


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


        currentPanel = panel;
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
