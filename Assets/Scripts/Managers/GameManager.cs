using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isTesting = false;
    public void TogleTestMode() => isTesting = !isTesting;

    public LevelManager levelManager { private set; get;}
    public UiManager uiManager {private set; get;}
    public ScoreManager scoreManager {private set; get;}
    public MenuTransitionController menuTranstionController {private set; get;}
    public PlayerController playerController {private set; get;}
    public RewardManager rewardManager {private set; get;}
    public PlayerData playerData {private set; get;}

    public EGameState gameState {private set; get;} = EGameState.inMenu;

    private EPanel currentPanelOnProgress;
    private EGameState currentGameStateInProgress;

    private int currentLevelIndex = 0;

    public bool isVibrateOn = true;

    public int currentLeaderBoardRank { private set; get; } = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        playerData = SaveManager.instance.data;


        levelManager = FindFirstObjectByType<LevelManager>();
        uiManager = FindFirstObjectByType<UiManager>();
        menuTranstionController = FindFirstObjectByType<MenuTransitionController>();
        playerController = FindFirstObjectByType<PlayerController>();
        scoreManager = FindFirstObjectByType<ScoreManager>();
        rewardManager = FindFirstObjectByType<RewardManager>();
    }

    private void Start()
    {
        gameState = EGameState.inMenu;
        uiManager?.SetPanel(EPanel.MainMenu);
        playerController.SetGameStarted(false);

        if (isTesting)
        {
            StartGame();
            levelManager.Test(0);
        }
    }

    private void CallGameStart()
    {
        playerController.SetGameStarted(true);
        levelManager.SetGameStart();
        scoreManager.ResetAll();
        rewardManager.ResetRewards();
    }

    private void CallGameOver()
    {

        playerController.SetGameStarted(false);
        levelManager.SetGameEnd();
    }

    private void ChangeGameState(EGameState state)
    {

        switch (state)
        {
            case EGameState.inMenu:
                if (gameState == EGameState.pause)
                {
                    Time.timeScale = 1;
                    CallGameOver();
                }

                else if (gameState == EGameState.InGame)
                {
                    Debug.Log("This Should Never Happens now !!!");

                    Time.timeScale = 1;
                    CallGameOver();
                }
                gameState = EGameState.inMenu;
                break;
            case EGameState.InGame:
                Time.timeScale = 1;

                if (gameState == EGameState.inMenu)
                    CallGameStart();

                gameState = EGameState.InGame;
                break;
            case EGameState.pause:
                Time.timeScale = 0;
                gameState = EGameState.pause;
                break;
        }
    }

    public void StartGame()
    {
        uiManager?.SetPanel(EPanel.None);
        currentPanelOnProgress = EPanel.InGame;
        currentGameStateInProgress = EGameState.InGame;

        if (menuTranstionController != null)
        {
            menuTranstionController.EnterTheLevel();
            return;
        }

        OnMenuTransitionFinished();
    }

    public void RestartGame()
    {
        uiManager?.SetPanel(EPanel.None);
        currentPanelOnProgress = EPanel.InGame;
        currentGameStateInProgress = EGameState.InGame;

        SaveGameResults();
        CallGameOver();
        CallGameStart();

        OnMenuTransitionFinished();
    }

    public void OnLastChance()
    {
        uiManager.SetPanel(EPanel.None);
        currentPanelOnProgress = EPanel.LastChance;
        currentGameStateInProgress = EGameState.pause;

        OnMenuTransitionFinished();
    }

    public void ReviveThePlayer()
    {
        scoreManager.ClearHostageKill();
        playerController.ReviveThePlayer();

        OnResume();
    }

    public void GameOver()
    {
        uiManager?.SetPanel(EPanel.None);
        currentPanelOnProgress = EPanel.GameOver;
        currentGameStateInProgress = EGameState.inMenu;

        playerController.OnGameOver();

        SaveGameResults();

        if (menuTranstionController != null)
            menuTranstionController.LeaveScene();

        OnMenuTransitionFinished();
    }

    public void LevelCompleted()
    {
        uiManager?.SetPanel(EPanel.None);
        currentPanelOnProgress = EPanel.Victory;
        currentGameStateInProgress = EGameState.inMenu;

        SaveGameResults(true);

        if (menuTranstionController != null && gameState == EGameState.inMenu)
        {
            menuTranstionController.LeaveScene();
            return;
        }

        OnMenuTransitionFinished();
    }

    public void OnMenuTransitionFinished()
    {
        uiManager?.SetPanel(currentPanelOnProgress);
        ChangeGameState(currentGameStateInProgress);
    }

    public GameObject GetUiPanel(EPanel panel) => uiManager.GetPanel(panel);

    public void OnPause()
    {

        currentPanelOnProgress = EPanel.Pause;
        currentGameStateInProgress = EGameState.pause;

        OnMenuTransitionFinished();
    }

    public void OnResume()
    {
        currentPanelOnProgress = EPanel.InGame;
        currentGameStateInProgress = EGameState.InGame;

        OnMenuTransitionFinished();
    }

    public void LevelSelection()
    {
        if (uiManager == null) return;
        currentPanelOnProgress = EPanel.LevelSelection;
        currentGameStateInProgress = EGameState.inMenu;

        if (menuTranstionController != null && gameState == EGameState.inMenu)
        {
            uiManager.FadePanel(menuTranstionController.actionTime * .9f);
            menuTranstionController.ChangeSideViewAnimation();
            return;
        }
        OnMenuTransitionFinished();
    }

    public void OnLevelSelected()
    {
        uiManager?.SetPanel(EPanel.None);
        currentPanelOnProgress = EPanel.Ready;

        if (menuTranstionController != null && gameState == EGameState.inMenu)
        {
            menuTranstionController.LevelView();
            return;
        }
        OnMenuTransitionFinished();
    }
    public void MainMenuFromLevelScene()
    {
        
        uiManager?.SetPanel(EPanel.MainMenu);
    }
    public void MainMenuFromSideView()
    {
        uiManager?.SetPanel(EPanel.None);
        currentPanelOnProgress = EPanel.MainMenu;
        currentGameStateInProgress = EGameState.inMenu;

        if (menuTranstionController != null && gameState == EGameState.inMenu)
        {
            menuTranstionController.LevelView();
            return;
        }

        OnMenuTransitionFinished();
    }

    public void StartNextLevel()
    {
        uiManager?.SetPanel(EPanel.None);
        uiManager.FadePanel();

        if (levelManager.LoadNextLevel() == false)
        {
            LevelSelection();
            return;
        }

        currentPanelOnProgress = EPanel.Ready;
        currentGameStateInProgress = EGameState.inMenu;

        if (menuTranstionController != null && gameState == EGameState.inMenu)
        {
            menuTranstionController.ChangeAndEnterNextLevel();
            return;
        }

        OnMenuTransitionFinished();
    }

    public void OnSettingClick()
    {
        uiManager?.SetPanel(EPanel.None);
        currentPanelOnProgress = EPanel.Settings;

        if (menuTranstionController != null && gameState == EGameState.inMenu)
        {
            menuTranstionController.ChangeSideViewAnimation();
            return;
        }

        OnMenuTransitionFinished();
    }

    public void OnLeaderBoardClick()
    {
        uiManager?.SetPanel(EPanel.None);
        currentPanelOnProgress = EPanel.LeaderBoard;

        if (menuTranstionController != null && gameState == EGameState.inMenu)
        {
            menuTranstionController.ChangeSideViewAnimation();
            return;
        }

        OnMenuTransitionFinished();
    }
    public void CreditWindow()
    {
        uiManager?.SetPanel(EPanel.None);
        currentPanelOnProgress = EPanel.Credits;

        if (menuTranstionController != null && gameState == EGameState.inMenu)
        {
            menuTranstionController.ChangeSideViewAnimation();
            return;
        }

        OnMenuTransitionFinished();
    }

    public void ShareBack()
    {
        if (gameState == EGameState.inMenu)
            MainMenuFromLevelScene();
        if (gameState == EGameState.pause)
            OnPause();
    }

    private void SaveGameResults(bool isCompleted = false)
    {
        float points = scoreManager.points;
        int levelIndex = levelManager.currentLevelIndex;

        if (playerData.levelPoints[levelIndex] < points)
            playerData.levelPoints[levelIndex] = points;

        int earnedStars = CheckLevelStarsEarned();
        if (playerData.levelStars[levelIndex] < earnedStars)
            playerData.levelStars[levelIndex] = earnedStars;

        if (isCompleted)
            if (playerData.levelStars[levelIndex] > 0)
                if (playerData.unlockedLevel < currentLevelIndex + 1)
                    playerData.unlockedLevel = currentLevelIndex + 1;

        playerData.totalPoints += points;

        SaveManager.instance.Save();
        SaveManager.instance.AddLeaderBoardEntry(playerData.name, points,
            scoreManager.currentKills, levelManager.GetLevelName(levelIndex), out int rank);
        currentLeaderBoardRank = rank;
    }

    public int CheckLevelStarsEarned()
    {
        float[] requirepoints =levelManager.currentLevelStarPoints;
        if (requirepoints == null || requirepoints.Length == 0)
            return 0;

        for (int i = 0; i < requirepoints.Length; i++)
        {
       
            if (scoreManager.points < requirepoints[i])
                return i;
        }

        return requirepoints.Length;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnApplicationQuit()
    {
        playerData.lastGunId = playerController.gunManager.currentGun.gunID;

        SaveManager.instance.Save();
    }


}
