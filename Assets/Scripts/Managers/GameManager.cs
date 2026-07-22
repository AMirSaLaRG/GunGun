using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public LevelManager levelManager { private set; get; }
    public UiManager uiManager {private set; get;}
    public ElevatorMainMenu elevator {private set; get;}
    public PlayerController playerController {private set; get;}
    public PlayerData playerData {private set; get;}


    private int currenLevelIndex = 0;

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
        uiManager.SetPanel(EPanel.MainMenu);
        playerController.SetGameStarted(false);

        playerData = SaveManager.instance.data;
    }


    public void LevelSelection()
    {
        uiManager.SetPanel(EPanel.None);
        levelManager.PopulateLevels();

        StartCoroutine(levelSelectionCo());
    }
    private IEnumerator levelSelectionCo()
    {
        elevator.ChangeLevelAnimation();
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

    public void MainMenuFromLevelView()
    {
        uiManager.SetPanel(EPanel.MainMenu);

    }
    public void MainMenuFromLevelSelection()
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
        uiManager.SetPanel(EPanel.None);
        StartCoroutine(StartGameCo());

    }

    private IEnumerator StartGameCo()
    {
        elevator.EnterTheLevel();
        playerController.ResetPlayer();
        yield return new WaitForSeconds(elevator.actionTime * 2);
        uiManager.SetPanel(EPanel.InGame);
        playerController.SetGameStarted(true);
        levelManager.SetGameStart(true);
    }

    public void SetToNextLevel()
    {
        uiManager.SetPanel(EPanel.None);
        StartCoroutine(SetToNextLevelCo());
    }

    private IEnumerator SetToNextLevelCo()
    {
        elevator.ChangeLevelAnimation();
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
        uiManager.SetPanel(EPanel.None);
        StartCoroutine(LevelCompletedCo());
    }

    private IEnumerator LevelCompletedCo()
    {
        elevator.LeaveScene();
        yield return new WaitForSeconds(elevator.actionTime * 2);
        uiManager.SetPanel(EPanel.Victory);
        playerController.SetGameStarted(false);
        levelManager.SetGameStart(false);
        levelManager.ClearScene();


        SaveLevelCompleted();
    }
    public void GameOver()
    {
        uiManager.SetPanel(EPanel.None);
        levelManager.ClearScene();
        StartCoroutine(GameOverCo());
    }

    private IEnumerator GameOverCo()
    {
        elevator.LeaveScene();
        yield return new WaitForSeconds(elevator.actionTime * 2);
        uiManager.SetPanel(EPanel.GameOver);
        playerController.SetGameStarted(false);

        levelManager.SetGameStart(false);
        levelManager.ClearScene();


        SaveGameOver();
    }

    private void SaveLevelCompleted()
    {
        
        float points = playerController.points;
        int currentLevelIndex = levelManager.currentLevelIndex;

        if (playerData.levelPoints[currenLevelIndex] < points)
            playerData.levelPoints[currentLevelIndex] = points;

        //starts
 
        if (playerData.unlockedLevel < currentLevelIndex + 1)
            playerData.unlockedLevel = currentLevelIndex + 1;

        SaveManager.instance.Save();
    }
    private void SaveGameOver()
    {
        float points = playerController.points;
        int currentLevelIndex = levelManager.currentLevelIndex;

        playerData.levelPoints[currentLevelIndex] = points;
 

        SaveManager.instance.Save();
    }
}
