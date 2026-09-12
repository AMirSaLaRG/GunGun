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
            data.name = "Player";
            data.totalPoints = 0;
            data.soldirCoins = 0;
            data.playerLevelsData = new PlayersLevelData[0];
            data.unlockedWeapons = new int[] { 0 };
            data.musicVolume = 1f;
            data.sfxVolume = 1f;
            data.lastGunId = 0;

            Save();
        }
    }

    public PlayersLevelData GetLevelData(EBuilding building, int stage, int levelIndex)
    {
        if (data.playerLevelsData == null) return null;

        foreach (var level in data.playerLevelsData)
        {
            if (level != null &&
                level.Building == building &&
                level.buildingStage == stage &&
                level.levelIndex == levelIndex)
            {
                return level;
            }
        }
        return null;
    }

    public int GetStars(EBuilding building, int stage, int levelIndex)
    {
        var level = GetLevelData(building, stage, levelIndex);
        return level != null ? level.stars : 0;
    }
    public float GetPoints(EBuilding building, int stage, int levelIndex)
    {
        var level = GetLevelData(building, stage, levelIndex);
        return level != null ? level.points : 0f;
    }

    [ContextMenu("DeleteSave")]
    private void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        data = new PlayerData
        {
            name = "Player",
            totalPoints = 0,
            soldirCoins = 0,
            playerLevelsData = new PlayersLevelData[0],
            unlockedWeapons = new int[] { 0 }, // Default starting weapon
            musicVolume = 1.0f,
            sfxVolume = 1.0f,
            lastGunId = 0
        };

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

    public void SaveLevelProgress(EBuilding building, int stage, int levelIndex, int starsEarned, float score)
    {
        PlayersLevelData level = GetLevelData(building, stage, levelIndex);

        if (level == null)
        {
            level = new PlayersLevelData
            {
                Building = building,
                buildingStage = stage,
                levelIndex = levelIndex,
                isLock = false
            };

            Array.Resize(ref data.playerLevelsData, data.playerLevelsData.Length + 1);
            data.playerLevelsData[data.playerLevelsData.Length - 1] = level;
        }

        if (score > level.points) level.points = score;
        if (starsEarned > level.stars) level.stars = starsEarned;

        Save();
    }

    public void UnlockLevel(EBuilding building, int stage, int levelIndex)
    {
        PlayersLevelData level = GetLevelData(building, stage, levelIndex);

        if (level != null)
        {
            level.isLock = false;
        }
        else
        {
            PlayersLevelData newLevel = new PlayersLevelData
            {
                Building = building,
                buildingStage = stage,
                levelIndex = levelIndex,
                isLock = false
            };

            Array.Resize(ref data.playerLevelsData, data.playerLevelsData.Length + 1);
            data.playerLevelsData[data.playerLevelsData.Length - 1] = newLevel;
        }

        Save();
    }
}
