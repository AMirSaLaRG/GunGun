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

    private float lastTimeScale = 1f;

    private bool isSurvival;

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

    public void ChangeTimescale(float timeScale)
    {
        lastTimeScale = timeScale;

        if (currentGameStateInProgress == EGameState.InGame)
            Time.timeScale = timeScale;
    }

    private void CallGameStart()
    {
        AudioListener.pause = false;
        AudioManager.instance.StopAllSfxInScene();

        playerController.SetGameStarted(true);


        levelManager.StartLevel();


        scoreManager.ResetAll();
        rewardManager.ResetRewards();
        lastTimeScale = 1;
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
{
                if (gameState == EGameState.pause ||
                    gameState == EGameState.InGame)
                {
                    Time.timeScale = 1f;
                    CallGameOver();

                }

                gameState = EGameState.inMenu;

                if (AudioManager.instance != null)
                    {
                        AudioListener.pause = false;
                        AudioManager.instance.StopAllSfxInScene();
                        AudioManager.instance.PlayRandomBGM(EGameState.inMenu);
                    }
                

                break;
            }

            case EGameState.InGame:
                Time.timeScale = lastTimeScale;

                if (gameState == EGameState.inMenu)
                {
                    CallGameStart();
                    AudioManager.instance.PlayRandomBGM(state);

                }

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
        if (isTesting)
            return;
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

        playerController.OnPause();

        OnMenuTransitionFinished();
    }

    public void OnResume()
    {
        pausedBySystem = false;
        AudioListener.pause = false;
        playerController.OnResume();


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
        currentPanelOnProgress = EPanel.MainMenu;
        currentGameStateInProgress= EGameState.inMenu;

        if (menuTranstionController != null && gameState == EGameState.inMenu)
        {
            menuTranstionController.LevelView();
            return;
        }

        OnMenuTransitionFinished();
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

        StartGame();
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

        if (LevelManager.instance == null)
        {
            Debug.LogError("Cannot save game results because LevelManager.instance is missing.");
            return;
        }

        if (SaveManager.instance == null)
        {
            Debug.LogError("Cannot save game results because SaveManager.instance is missing.");
            return;
        }

        bool isSurvival = LevelManager.instance.isSurvival;

        if (isSurvival)
        {
            SaveToLeaderBoard();

        }
        else
        {
            int earnedStars = CheckLevelStarsEarned();

            SaveManager.instance.SaveLevelProgress(
                building,
                buildingStage,
                levelIndex,
                earnedStars,
                points
            );

            if (isCompleted && earnedStars > 0)
            {
                int nextLevelIndex = levelIndex + 1;

                SaveManager.instance.UnlockLevel(
                    building,
                    buildingStage,
                    nextLevelIndex
                );
            }

            SaveManager.instance.data.totalPoints += points;
            SaveManager.instance.Save();
        }

        
    }
    internal void UpdateNameChangeLeaderBoard()
    {
        if (SaveManager.instance == null)
        {
            Debug.LogError("Cannot update leaderboard because SaveManager.instance is missing.");
            return;
        }

        if (currentLeaderBoardRank <= 0)
        {
            Debug.LogWarning("Cannot update leaderboard because the saved rank is invalid.");
            return;
        }

        string playerName = SaveManager.instance.data.name;

        if (string.IsNullOrWhiteSpace(playerName))
        {
            Debug.LogWarning("Player name is empty. Not updating leaderboard.");
            OpenPlayerForm();
            return;
        }

        SaveManager.instance.RemoveLeaderBoardEntry(currentLeaderBoardRank);

        currentLeaderBoardRank = 0;
        SaveToLeaderBoard();
    }
    public void SaveToLeaderBoard()
    {

        if (SaveManager.instance == null)
        {
            Debug.LogError("Cannot save game results because SaveManager.instance is missing.");
            return;
        }

        float points = scoreManager.points;

        if (points <= 0)
        {
            Debug.LogWarning("Points are zero or negative. Not saving to leaderboard.");
            return;
        }



        SaveManager.instance.AddLeaderBoardEntry(
                        SaveManager.instance.data.name,
                        points,
                        scoreManager.currentKills,
                        buildingManager.currentBuilding.ToString(),
                        out int rank
                    );

        currentLeaderBoardRank = rank;

        SaveManager.instance.data.totalPoints += points;
        SaveManager.instance.Save();

        uiManager?.PopDynamicPanel(EDPanel.LeaderBoard);

    }

    private void OpenPlayerForm()
    {
        uiManager?.PopDynamicPanel(EDPanel.PlayerForm);
    }

    public void OnPlayerFormSubmitted(string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("Player name is empty. Not saving to leaderboard.");
            return;
        }
        SaveManager.instance.data.name = playerName;
        SaveManager.instance.Save();
        SaveToLeaderBoard();
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

    public void ResetLeaderBoardRank()
    {
        currentLeaderBoardRank = 0;
    }
}


