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
    public BuildingManager buildingManager { private set; get;}
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

    private bool pausedBySystem = false;
    public int currentLeaderBoardRank { private set; get; } = 0;
    [Header("Sfx")]
    [SerializeField] private AudioSource missionCompeletedSfx;
    [SerializeField] private AudioSource missionFailedSfx;
    [SerializeField] private AudioSource reviveSfx;
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
        buildingManager = FindFirstObjectByType<BuildingManager>();
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

        AudioManager.instance.PlaySfx(reviveSfx);

        OnResume();
    }

    public void GameOver()
    {
        uiManager?.SetPanel(EPanel.None);
        currentPanelOnProgress = EPanel.GameOver;
        currentGameStateInProgress = EGameState.inMenu;

        playerController.OnGameOver();

        AudioManager.instance.PlaySfx(missionFailedSfx);

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

        AudioManager.instance.PlaySfx(missionCompeletedSfx);

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

        AudioListener.pause = true;


        OnMenuTransitionFinished();
    }

    public void OnResume()
    {
        pausedBySystem = false;
        AudioListener.pause = false;

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
            MainMenuFromLevelScene();
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

    //public void OnSettingClick()
    //{
    //    uiManager?.SetPanel(EPanel.None);
    //    currentPanelOnProgress = EPanel.Settings;

    //    if (menuTranstionController != null && gameState == EGameState.inMenu)
    //    {
    //        menuTranstionController.ChangeSideViewAnimation();
    //        return;
    //    }

    //    OnMenuTransitionFinished();
    //}

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
        EBuilding building = buildingManager.currentBuilding;
        int buildingStage = buildingManager.currentBuildingStage;

        // Calculate stars earned on this run
        int earnedStars = CheckLevelStarsEarned();

        // 1. Save or update progress for the current level (points and stars)
        SaveManager.instance.SaveLevelProgress(building, buildingStage, levelIndex, earnedStars, points);

        // 2. Unlock the next level index if this level was completed with at least 1 star
        if (isCompleted && earnedStars > 0)
        {
            int nextLevelIndex = levelIndex + 1;

            // Unlocks the next level index in the same building stage
            SaveManager.instance.UnlockLevel(building, buildingStage, nextLevelIndex);
        }

        // 3. Add to total career points
        SaveManager.instance.data.totalPoints += points;

        // 4. Save changes to disk
        SaveManager.instance.Save();

        // 5. Leaderboard entry submission
        SaveManager.instance.AddLeaderBoardEntry(
            SaveManager.instance.data.name,
            points,
            scoreManager.currentKills,
            levelManager.GetLevelName(levelIndex),
            out int rank
        );

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
        //playerData.lastGunId = playerController.gunManager.currentGun.gunID;

        SaveManager.instance.Save();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            HandleAppBackgrounded();
        else
            HandleAppForegrounded();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        // Only trust focus events on mobile; on desktop they fire on alt-tab.
        if (!Application.isMobilePlatform)
            return;

        if (!hasFocus)
            HandleAppBackgrounded();
        else
            HandleAppForegrounded();
    }

    private void HandleAppBackgrounded()
    {
        if (pausedBySystem) return;   // already handled, avoid double-fire

        // Safety save: OS may kill us while backgrounded
        SaveManager.instance.Save();

        // Only pause if we're actually in gameplay
        if (gameState == EGameState.InGame)
        {
            pausedBySystem = true;
            OnPause();                // shows Pause panel, sets timeScale = 0
        }

        // Freeze audio regardless of state
        AudioListener.pause = true;
    }

    private void HandleAppForegrounded()
    {
        AudioListener.pause = false;
        // Do NOT auto-resume — player taps Resume.
    }

}
