using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LeaderBoardData
{
    public string name;
    public float points;
    public string date;
    public int kills;
    public string levelName;
}

[System.Serializable]
public class LeaderBoardDataListWrapper
{
    public List<LeaderBoardData> list = new List<LeaderBoardData>();
}
