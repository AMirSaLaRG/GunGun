using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    private string savePath;
    public PlayerData data;
    private string leaderBoardSavePath;
    public List<LeaderBoardData> leaderBoardDataList = new List<LeaderBoardData>();
    public int LeaderBoardMaxEnteries = 20;

    public static SaveManager instance;

    private void Awake()
    {
        instance = this;

        savePath = Application.persistentDataPath + "/PlayerData.json";
        leaderBoardSavePath = Application.persistentDataPath + "/LeaderBoardData.json";
        Load();
        LoadLeaderBoard();
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    private void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            data = JsonUtility.FromJson<PlayerData>(json);
        } else
        {
            data = new PlayerData();
            data.unlockedLevel = 0;
            data.levelStars = new int[100];
            data.levelPoints = new float[100];
        }
    }

    public int GetStarts(int levelIndex) => data.levelStars[levelIndex];
    public float GetPoints(int levelIndex) => data.levelPoints[levelIndex];

    [ContextMenu("DeleteSave")]
    private void DeleteSave()
    {
        data = new PlayerData();
        data.unlockedLevel = 0;
        data.levelStars = new int[100];
        data.levelPoints = new float[100];

        Save();
    }

    private void SaveLeaderBoard()
    {
        OrganizeTheLeaderBoard();

        LeaderBoardDataListWrapper wrapper = new LeaderBoardDataListWrapper();
        wrapper.list = leaderBoardDataList;

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(leaderBoardSavePath, json);
    }

    private void LoadLeaderBoard()
    {
        if (File.Exists(leaderBoardSavePath))
        {
            string json = File.ReadAllText(leaderBoardSavePath);
            LeaderBoardDataListWrapper wrapper = JsonUtility.FromJson<LeaderBoardDataListWrapper>(json);

            leaderBoardDataList = wrapper.list ?? new List<LeaderBoardData>();

            OrganizeTheLeaderBoard();

        }
        else
        {
            leaderBoardDataList = new List<LeaderBoardData>();
        }
    }

    private void OrganizeTheLeaderBoard()
    {
        leaderBoardDataList.Sort((a, b) => b.points.CompareTo(a.points));

        if (LeaderBoardMaxEnteries < leaderBoardDataList.Count)
        {
            leaderBoardDataList.RemoveRange(LeaderBoardMaxEnteries, leaderBoardDataList.Count - LeaderBoardMaxEnteries);
        }
    }

    public void AddLeaderBoardEntry(string name, float points, int kills, string levelName, out int rank)
    {
        LeaderBoardData newEntry = new LeaderBoardData
        {
            name = name,
            points = points,
            date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),  // ISO format like Python
            kills = kills,
            levelName = levelName
        };

        leaderBoardDataList.Add(newEntry);
        SaveLeaderBoard(); // This will organize and save
        if (leaderBoardDataList.Contains(newEntry))
            rank = leaderBoardDataList.IndexOf(newEntry) + 1;
        else rank = 0;
    }

    [ContextMenu("ClearLeaderBoard")]
    private void ClearLeaderBoard()
    {
        leaderBoardDataList.Clear();
        SaveLeaderBoard();
    }
}
