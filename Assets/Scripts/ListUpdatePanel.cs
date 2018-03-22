using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

//Class to check how many users updated their score in a time window (and how much updates for each of them)
public class ListUpdatePanel : AdminRequestPanel
{
    public Button requestButton;

    public InputField startTimeInput;
    public InputField endTimeInput;

    public GameObject resultContentPanel;
    public GameObject resultItemPrefab;

    private List<NameWithData> cachedItems;

    private void Awake()
    {
        cachedItems = new List<NameWithData>();
    }

    public void Request()
    {
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        DateTime res;

        //Parse start timestamp from text
        string startTimeStr = startTimeInput.text;
        if (!DateTime.TryParseExact(startTimeStr, "MM/dd/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.AssumeLocal, out res)) return;

        int startTime = (int)((res.ToUniversalTime() - epoch).TotalSeconds);

        //Parse end timestamp from text
        string endTimeStr = endTimeInput.text;
        if (!DateTime.TryParseExact(endTimeStr, "MM/dd/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.AssumeLocal, out res)) return;

        int endTime = (int)((res.ToUniversalTime() - epoch).TotalSeconds);

        if (endTime <= startTime) return;

        requestButton.gameObject.SetActive(false);

        //Request data from server
        string url = GetBaseURL() + "/update_log/" + startTime.ToString() + "-" + endTime.ToString();
        StartCoroutine(Request_Coroutine(url, "GET", OnSuccess, OnFailed));
    }

    public void OnSuccess(string res)
    {
        int numCachedItems = cachedItems.Count;
        int itemId = 0;

        //Parse pairs of user name / number of updates from JSON response from server 
        JSONObject root = SimpleJSON.JSON.Parse(res) as JSONObject;
        foreach (KeyValuePair<string, JSONNode> pair in root)
        {
            string name = pair.Key;
            int count = pair.Value.AsInt;

            if (itemId >= numCachedItems)
            {
                GameObject newObj = GameObject.Instantiate(resultItemPrefab, resultContentPanel.transform);
                cachedItems.Add(newObj.GetComponent<NameWithData>());

                numCachedItems++;
            }

            //Populate the UI object with the data
            cachedItems[itemId++].Init(name, count.ToString());
        }

        //Remove unused cached UI objects
        if (itemId < numCachedItems)
        {
            for (int i = itemId; i < numCachedItems; ++i)
            {
                GameObject.Destroy(cachedItems[i].gameObject);
            }

            cachedItems.RemoveRange(itemId, numCachedItems - itemId);
        }
        
        requestButton.gameObject.SetActive(true);
    }

    public void OnFailed(long code, string err)
    {
        //Clear all UI objects
        int numCachedItems = cachedItems.Count;

        for (int i = 0; i < numCachedItems; ++i)
        {
            GameObject.Destroy(cachedItems[i].gameObject);
        }

        cachedItems.Clear();

        requestButton.gameObject.SetActive(true);
    }
}
