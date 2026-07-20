using System;
using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    private string savePath;
    public PlayerData data;

    public static SaveManager instance;

    private void Awake()
    {
        instance = this;

        savePath = Application.persistentDataPath + "/PlayerData.json";
        Load();
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
}
