using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardItemData
{
    public LeaderboardItemData(string name, int score)
    {
        this.name = name;
        this.score = score;
    }

    public string name;
    public int score;
}

//Class to store leaderboard data from server
public class LeaderboardData
{
    public List<LeaderboardItemData> items = new List<LeaderboardItemData>();

    public void AddItem(string name, int score)
    {
        items.Add(new LeaderboardItemData(name, score));
    }
}
