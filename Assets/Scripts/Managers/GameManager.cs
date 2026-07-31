using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isTesting = false;
    public void TogleTestMode() => isTesting = !isTesting;

    public LevelManager levelManager { private set; get; }
    public UiManager uiManager {private set; get;}
    public ElevatorMainMenu elevator {private set; get;}
    public PlayerController playerController {private set; get;}
    public PlayerData playerData {private set; get;}

    public EGameState gameState {private set; get;} = EGameState.inMenu;

    private int currenLevelIndex = 0;

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

        levelManager = FindFirstObjectByType<LevelManager>();
        uiManager = FindFirstObjectByType<UiManager>();
        elevator = FindFirstObjectByType<ElevatorMainMenu>();
        playerController = FindFirstObjectByType<PlayerController>();
    }

    private void Start()
    {
        gameState = EGameState.inMenu;
        uiManager.SetPanel(EPanel.MainMenu);
        playerController.SetGameStarted(false);

        playerData = SaveManager.instance.data;

        if (isTesting)
        {
            StartGame();
            levelManager.Test(0);
        }
    }


    public void LevelSelection()
    {
        uiManager.FadePanel(elevator.actionTime * .9f);
        levelManager.PopulateLevels();

        StartCoroutine(levelSelectionCo());
    }
    private IEnumerator levelSelectionCo()
    {
        
        elevator.ChangeSideViewAnimation();
        yield return new WaitForSeconds(elevator.actionTime);
        uiManager.SetPanel(EPanel.LevelSelection);
    }

    public void OnLevelSelected()
    {
        uiManager.SetPanel(EPanel.None);

        StartCoroutine(OnLevelSelectedCo());
    }

    private IEnumerator OnLevelSelectedCo()
    {
        elevator.SceneChangeAnim();
        yield return new WaitForSeconds(elevator.actionTime * (.6f));
        elevator.LevelView();
        yield return new WaitForSeconds(elevator.actionTime);

        uiManager.SetPanel(EPanel.Ready);


    }


    public void MainMenuFromLevelScene()
    {
        uiManager.SetPanel(EPanel.MainMenu);
    }

    public void MainMenuFromSideView()
    {
        uiManager.SetPanel(EPanel.None);
        StartCoroutine(MainMenuFromLevelSelectionCo());
    }
    private IEnumerator MainMenuFromLevelSelectionCo()
    {
        elevator.LevelView();
        yield return new WaitForSeconds(elevator.actionTime);
        uiManager.SetPanel(EPanel.MainMenu);
    }

    public void StartGame()
    {
        gameState = EGameState.inGame;
        uiManager.SetPanel(EPanel.None);
        StartCoroutine(StartGameCo());

    }

    private IEnumerator StartGameCo()
    {
        elevator.EnterTheLevel();
        yield return new WaitForSeconds(elevator.actionTime * 2);

        uiManager.SetPanel(EPanel.InGame);
        playerController.ResetPlayer();

        playerController.SetGameStarted(true);
        levelManager.SetGameStart();
    }

    public void SetToNextLevel()
    {
        uiManager.SetPanel(EPanel.None);
        StartCoroutine(SetToNextLevelCo());
    }

    private IEnumerator SetToNextLevelCo()
    {
        elevator.ChangeSideViewAnimation();
        yield return new WaitForSeconds(elevator.actionTime);
        levelManager.LoadNextLevel();
        elevator.SceneChangeAnim();
        yield return new WaitForSeconds(elevator.actionTime * (.6f));
        elevator.LevelView();
        yield return new WaitForSeconds(elevator.actionTime);
        uiManager.SetPanel(EPanel.Ready);

    }

    public void LevelCompleted()
    {
        gameState = EGameState.inMenu;
        uiManager.SetPanel(EPanel.None);
        StartCoroutine(LevelCompletedCo());
    }

    private IEnumerator LevelCompletedCo()
    {
        elevator.LeaveScene();
        yield return new WaitForSeconds(elevator.actionTime * 2);
        playerController.SetGameStarted(false);
        levelManager.SetGameEnd();

        uiManager.SetPanel(EPanel.Victory);


        SaveLevelCompleted();
    }
    public void GameOver()
    {
        gameState = EGameState.inMenu;
        uiManager.SetPanel(EPanel.None);
        StartCoroutine(GameOverCo());
    }

    private IEnumerator GameOverCo()
    {
        elevator.LeaveScene();
        yield return new WaitForSeconds(elevator.actionTime * 2);
        uiManager.SetPanel(EPanel.GameOver);
        playerController.SetGameStarted(false);
        levelManager.SetGameEnd();


        SaveGameOver();
    }

    public void OnSettingClick()
    {
        uiManager.SetPanel(EPanel.Settings);

        //uiManager.SetPanel(EPanel.None);
        //StartCoroutine(OnSettingClickCo());
    }

    private IEnumerator OnSettingClickCo()
    {
        elevator.ChangeSideViewAnimation();
        yield return new WaitForSeconds(elevator.actionTime);
        uiManager.SetPanel(EPanel.Settings);
    }
    public void OnLeaderBoardClick()
    {
        uiManager.SetPanel(EPanel.LeaderBoard);

        //uiManager.SetPanel(EPanel.None);
        //StartCoroutine(OnLeaderBoardClickCo());
    }

    private IEnumerator OnLeaderBoardClickCo()
    {
        elevator.ChangeSideViewAnimation();
        yield return new WaitForSeconds(elevator.actionTime);
        uiManager.SetPanel(EPanel.LeaderBoard);
    }

    public void CreaditWindow()
    {
        uiManager.SetPanel(EPanel.Credits);


        //uiManager.SetPanel(EPanel.None);
        //StartCoroutine(CreaditWindowCo());

    }

    private IEnumerator CreaditWindowCo()
    {
        elevator.ChangeSideViewAnimation();
        yield return new WaitForSeconds(elevator.actionTime);
        uiManager.SetPanel(EPanel.Credits);
    }
    private void SaveLevelCompleted()
    {
        
        float points = playerController.points;
        int currentLevelIndex = levelManager.currentLevelIndex;

        if (playerData.levelPoints[currentLevelIndex] < points)
            playerData.levelPoints[currentLevelIndex] = points;

        int earnedStart = ChecklevelStarsEarned();
        if (playerData.levelStars[currentLevelIndex] < earnedStart)
            playerData.levelStars[currentLevelIndex] = earnedStart;
 
        if (playerData.unlockedLevel < currentLevelIndex + 1)
            playerData.unlockedLevel = currentLevelIndex + 1;

        SaveManager.instance.Save();
        SaveManager.instance.AddLeaderBoardEntry(playerData.name, points, playerController.GetKills(), levelManager.GetLevelName(currentLevelIndex), out int rank);
        currentLeaderBoardRank = rank;

    }
    private void SaveGameOver()
    {
        float points = playerController.points;
        int currentLevelIndex = levelManager.currentLevelIndex;

        if (playerData.levelPoints[currenLevelIndex] < points)
            playerData.levelPoints[currentLevelIndex] = points;


        int earnedStart = ChecklevelStarsEarned();
        if (playerData.levelStars[currenLevelIndex] < earnedStart)
            playerData.levelStars[currenLevelIndex] = earnedStart;

        SaveManager.instance.Save();
        SaveManager.instance.AddLeaderBoardEntry(playerData.name, points, playerController.GetKills(), levelManager.GetLevelName(currentLevelIndex), out int rank);
        currentLeaderBoardRank = rank;

    }

    public int ChecklevelStarsEarned()
    {
        float[] requirepoints =levelManager.currentLevelStarPoints;
        if (requirepoints == null || requirepoints.Length == 0)
            return 0;



        for (int i = 0; i < requirepoints.Length; i++)
        {
       
            if (playerController.points < requirepoints[i])
                return i;
        }

        return 3;
    }




    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
