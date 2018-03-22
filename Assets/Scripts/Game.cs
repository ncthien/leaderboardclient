using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

//Main class for the client game
public class Game : MonoBehaviour
{
    private const int MAX_LEADERBOARD_ENTRY = 10;

    public UIManager uiManager;

    private string userName = string.Empty;
    private int userScore = 0;
    private int userHighScore = 0;

    private ConnectionManager.State lastConnectionState = ConnectionManager.State.CLOSED;

    //Queue to store actions that need to be executed in the main thread
    private List<Action> actionQueue = new List<Action>();
    private object actionQueueLock = new object();

    private LeaderboardData leaderboardData = null;
    private bool leaderboardDirty = false;

    void Start()
    {
        ConnectionManager.Instance.Init(this);
    }

    public void Update()
    {
        //Check changes in connection status to the webserver
        ConnectionManager.State connectionState = ConnectionManager.Instance.GetState();
        if (connectionState != lastConnectionState)
        {
            lastConnectionState = connectionState;

            if (connectionState == ConnectionManager.State.OPEN)
            {
                RequestScore();
                leaderboardDirty = true;
            }

            uiManager.UpdateConnectionButton(connectionState);
        }

        //Do actions in the queue
        lock (actionQueueLock)
        {
            if (actionQueue.Count > 0)
            {
                foreach (Action action in actionQueue)
                {
                    action();
                }

                actionQueue.Clear();
            }
        }

        //Request leaderboard if needed
        if (leaderboardDirty)
        {
            leaderboardDirty = false;
            RequestLeaderboard();
        }
    }

    //Check if the leaderboard needed to be updated with a new score
    private bool NeedUpdateLeaderboard(int score)
    {
        if (leaderboardData == null) return true;

        //Reload if the number items in the leaderboard stills less than the maximum number
        List<LeaderboardItemData> items = leaderboardData.items;
        if (items.Count < MAX_LEADERBOARD_ENTRY) return true;

        //Reload if the score is higher than the last item in the leaderboard
        if (items[items.Count - 1].score < score) return true;
        else return false;
    }

    public void SetUserName(string userName)
    {
        this.userName = userName;
    }

    public void SetUserScore(int userScore)
    {
        this.userScore = userScore;
    }

    //Update own highscore (not called from main thread)
    public void SetUserHighScore_Thread(int userHighScore)
    {
        lock (actionQueueLock)
        {
            actionQueue.Add(() => SetUserHighScore(userHighScore));
        }
    }

    //Update own highscore 
    public void SetUserHighScore(int userHighScore)
    {
        this.userHighScore = userHighScore;
        uiManager.SetUserHighScore(userHighScore);
    }

    //Get notified by a new highscore (not called from main thread)
    public void NotifyScore_Thread(string name, int score, int oldScore)
    {
        lock (actionQueueLock)
        {
            actionQueue.Add(() => NotifyScore(name, score, oldScore));
        }
    }

    //Get notified by a new highscore
    public void NotifyScore(string name, int score, int oldScore)
    {
        leaderboardDirty = NeedUpdateLeaderboard(score);
        uiManager.NotifyScore(name, score, oldScore);
    }

    //Update leaderboard data (not called from main thread)
    public void SetLeaderboard_Thread(LeaderboardData data)
    {
        lock (actionQueueLock)
        {
            actionQueue.Add(() => SetLeaderboard(data));
        }
    }

    //Update leaderboard data
    public void SetLeaderboard(LeaderboardData data)
    {
        leaderboardData = data;
        uiManager.UpdateLeaderboard(data);
    }

    //Request the leaderboard to websocket server
    public void RequestLeaderboard()
    {
        ConnectionManager.Instance.RequestLeaderboard(MAX_LEADERBOARD_ENTRY);
    }

    //Request the highscore to websocket server
    public void RequestScore()
    {
        ConnectionManager.Instance.RequestScore(userName);
    }

    //Send a score to websocket server
    public void SendScore()
    {
        //Make sure that the name is ASCII only
        name = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(name));
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("User name is invalid");
            return;
        }

        if (userScore <= 0)
        {
            Debug.LogError("User score is invalid");
            return;
        }

        ConnectionManager.Instance.SendScore(userName, userScore);
    }

    //Connect to websocket server
    public void Connect()
    {
        ConnectionManager.Instance.Connect();
    }

    //Disconnect from websocket server
    public void Disconnect()
    {
        ConnectionManager.Instance.Close();
    }
}
