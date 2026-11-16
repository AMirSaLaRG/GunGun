using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiTaskContent : MonoBehaviour
{
    public ETasks myTask;
    public DailyTaskManager myDailyTaskManager;
    public Image cheCkMark;
    public TextMeshProUGUI currentProgressText;
    public TextMeshProUGUI GoalProgressText;
    private void UpdateTasks()
    {
        switch (myTask)
        {
            case ETasks.Combo:
                UpdateComboTask();
                break;
            case ETasks.Kills:
                UpdateKillsTask();
                break;
        }
    }

    private void UpdateKillsTask()
    {
        currentProgressText.text = myDailyTaskManager.CurrentKills.ToString();

        GoalProgressText.text = myDailyTaskManager.KillsTarget.ToString();

        if (myDailyTaskManager.IsKillsTaskCompleted)
            cheCkMark.gameObject.SetActive(true);
        else 
            cheCkMark.gameObject.SetActive(false);

    }

    private void UpdateComboTask()
    {
        currentProgressText.text = myDailyTaskManager.CurrentCombo.ToString();

        GoalProgressText.text = myDailyTaskManager.ComboTarget.ToString();

        if (myDailyTaskManager.IsComboTaskCompleted)
            cheCkMark.gameObject.SetActive(true);
        else
            cheCkMark.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        UpdateTasks();
    }
}
