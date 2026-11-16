using System;
using System.Globalization;
using UnityEngine;


public class DailyTaskManager : MonoBehaviour
{
    public static DailyTaskManager instance;

    [Header("Daily Task Targets")]
    [SerializeField] private int comboTarget = 10;
    [SerializeField] private int killsTarget = 25;
    [SerializeField] private int taskRewardAmount = 1;

    [Header("Daily Visit Reward")]
    [SerializeField] private int dailyVisitRewardAmount = 1;

    public Transform taskContent;



    public int ComboTarget => comboTarget;
    public int KillsTarget => killsTarget;
    public int CurrentCombo => SaveManager.instance.data.dailyCombo;
    public int CurrentKills => SaveManager.instance.data.dailyKills;

    public int TaskRewardAmount => taskRewardAmount;
    public int DailyVisitRewardAmount => dailyVisitRewardAmount;

    public bool IsComboTaskCompleted => CurrentCombo >= comboTarget;
    public bool IsKillsTaskCompleted => CurrentKills >= killsTarget;
    public bool AreAllTasksCompleted => IsComboTaskCompleted && IsKillsTaskCompleted;

    public bool IsComboTaskRewardClaimed =>
        SaveManager.instance.data.dailyComboRewardClaimed;

    public bool IsKillsTaskRewardClaimed =>
        SaveManager.instance.data.dailyKillsRewardClaimed;

    public bool IsDailyVisitRewardClaimed =>
        SaveManager.instance.data.dailyVisitRewardClaimed;

    public bool CanClaimDailyVisitReward =>
        !IsDailyVisitRewardClaimed;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        CheckForNewDay();
    }

    public void CheckForNewDay()
    {
        if (SaveManager.instance == null || SaveManager.instance.data == null)
        {
            Debug.LogError("Daily tasks cannot start because SaveManager is missing.");
            return;
        }

        string today = GetTodayKey();

        if (SaveManager.instance.data.dailyTaskDate != today)
        {
            ResetTasks(today);
        }
    }

    public void RecordCombo(int combo)
    {
        CheckForNewDay();

        if (SaveManager.instance == null)
            return;

        if (combo > SaveManager.instance.data.dailyCombo)
        {
            SaveManager.instance.data.dailyCombo = combo;
            SaveManager.instance.Save();
        }

        TryGiveTaskReward(ETasks.Combo);
    }

    public void RecordKill()
    {
        CheckForNewDay();

        if (SaveManager.instance == null ||
            SaveManager.instance.data == null)
        {
            return;
        }

        if (SaveManager.instance.data.dailyKills < killsTarget)
        {
            SaveManager.instance.data.dailyKills++;
            SaveManager.instance.Save();
        }

        TryGiveTaskReward(ETasks.Kills);
    }

    public void ClaimDailyVisitReward()
    {
        CheckForNewDay();

        if (SaveManager.instance == null)
            return;

        if (SaveManager.instance.data.dailyVisitRewardClaimed)
        {
            Debug.Log("Daily visit reward has already been claimed.");
            return;
        }

        SaveManager.instance.AddSoldirCoins(dailyVisitRewardAmount);
        SaveManager.instance.data.dailyVisitRewardClaimed = true;
        SaveManager.instance.Save();

        Debug.Log(
            $"Daily visit reward claimed: {dailyVisitRewardAmount} soldir coin.");
    }
    public void ClaimKillsTaskReward()
    {
        CheckForNewDay();

        if (SaveManager.instance == null ||
            SaveManager.instance.data == null)
        {
            return;
        }

        if (!IsKillsTaskCompleted)
        {
            Debug.Log("Daily task requirements are not completed.");
            return;
        }

        if (IsKillsTaskRewardClaimed)
        {
            Debug.Log("Daily task reward has already been claimed.");
            return;
        }

        SaveManager.instance.AddSoldirCoins(taskRewardAmount);

        SaveManager.instance.data.dailyKillsRewardClaimed = true;
        SaveManager.instance.Save();

        Debug.Log(
            $"Daily task reward claimed: {taskRewardAmount} soldier coin.");
    }

    public void ClaimComboTaskReward()
    {
        CheckForNewDay();

        if (SaveManager.instance == null ||
            SaveManager.instance.data == null)
        {
            return;
        }

        if (!IsComboTaskCompleted)
        {
            Debug.Log("Daily task requirements are not completed.");
            return;
        }

        if (IsComboTaskRewardClaimed)
        {
            Debug.Log("Daily task reward has already been claimed.");
            return;
        }

        SaveManager.instance.AddSoldirCoins(taskRewardAmount);

        SaveManager.instance.data.dailyComboRewardClaimed = true;
        SaveManager.instance.Save();

        Debug.Log(
            $"Daily task reward claimed: {taskRewardAmount} soldier coin.");
    }

    private void TryGiveTaskReward(ETasks task)
    {
        switch (task)
        {
            case ETasks.Kills:
                if (IsKillsTaskCompleted && !IsKillsTaskRewardClaimed)
                {
                    ClaimKillsTaskReward();
                }
                break;
            case ETasks.Combo:
                if (IsComboTaskCompleted && !IsComboTaskRewardClaimed)
                {
                    ClaimComboTaskReward();
                }
                break;
        }
    }


    private void ResetTasks(string today)
    {
        SaveManager.instance.data.dailyTaskDate = today;
        SaveManager.instance.data.dailyCombo = 0;
        SaveManager.instance.data.dailyKills = 0;
        SaveManager.instance.data.dailyComboRewardClaimed = false;
        SaveManager.instance.data.dailyKillsRewardClaimed = false;
        SaveManager.instance.data.dailyVisitRewardClaimed = false;

        SaveManager.instance.Save();

        Debug.Log("Daily tasks and daily visit reward have been reset.");
    }

    private string GetTodayKey()
    {
        return DateTime.Now.Date.ToString(
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

}

public enum ETasks
{
    Kills = 0,
    Combo = 1,
}