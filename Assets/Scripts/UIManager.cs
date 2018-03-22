using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Class to handle UI for the client game
public class UIManager : MonoBehaviour
{
    public Game game;

    public InputField userNameInput;
    public InputField userScoreInput;

    public Text userHighScoreText;

    public Button connectButton;
    public Button disconnectButton;

    public NotifyMessagePanel notifyMessagePanel;

    public GameObject leaderboardContentPanel;
    public GameObject leaderboardItemPrefab;

    private List<NameWithData> leaderboardCachedItems;

    private void Awake()
    {
        leaderboardCachedItems = new List<NameWithData>();
    }

    public void Start()
    {
        //Load username from cached (if exists)
        if (PlayerPrefs.HasKey("username"))
        {
            string userName = PlayerPrefs.GetString("username");
            userNameInput.text = userName;
        }
    }

    public void OnChangeUserName()
    {
        string userName = userNameInput.text;
        game.SetUserName(userName);

        PlayerPrefs.SetString("username", userName);
        PlayerPrefs.Save();
    }

    public void OnChangeUserScore()
    {
        int score;
        if (int.TryParse(userScoreInput.text, out score))
        {
            game.SetUserScore(score);
        }
    }

    public void Connect()
    {
        game.Connect();
    }

    public void Disconnect()
    {
        game.Disconnect();
    }

    public void UpdateConnectionButton(ConnectionManager.State state)
    {
        if (state == ConnectionManager.State.CLOSED)
        {
            connectButton.gameObject.SetActive(true);
        }
        else
        {
            connectButton.gameObject.SetActive(false);
        }

        if (state == ConnectionManager.State.OPEN)
        {
            disconnectButton.gameObject.SetActive(true);
        }
        else
        {
            disconnectButton.gameObject.SetActive(false);
        }
    }

    public void SetUserHighScore(int userHighScore)
    {
        userHighScoreText.text = userHighScore.ToString();
    }

    public void NotifyScore(string name, int score, int oldScore)
    {
        notifyMessagePanel.Startup(name, score, oldScore);
    }

    //Populate UI elements with leaderboard data get from server
    public void UpdateLeaderboard(LeaderboardData data)
    {
        int numCachedItems = leaderboardCachedItems.Count;

        List<LeaderboardItemData> items = data.items;
        int numItems = items.Count;

        //Add new UI object if the cache doesn't have enough
        while (numItems > numCachedItems)
        {
            GameObject newObj = GameObject.Instantiate(leaderboardItemPrefab, leaderboardContentPanel.transform);
            leaderboardCachedItems.Add(newObj.GetComponent<NameWithData>());

            numCachedItems++;
        }

        //Remove unused cached UI objects
        if (numCachedItems > numItems)
        {
            for (int i = numItems; i < numCachedItems; ++i)
            {
                GameObject.Destroy(leaderboardCachedItems[i].gameObject);
            }

            leaderboardCachedItems.RemoveRange(numItems, numCachedItems - numItems);
        }

        for (int i = 0; i < numItems; ++i)
        {
            //Populate the UI object with the data
            LeaderboardItemData item = items[i];
            leaderboardCachedItems[i].Init(item.name, item.score.ToString());
        }
    }
}
