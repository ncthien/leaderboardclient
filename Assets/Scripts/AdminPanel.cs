using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

//Class to handle all admin tasks
public class AdminPanel : MonoBehaviour
{
    private int numTabs;

    public Button[] tabButtons;
    public GameObject[] panels;

    private void Awake()
    {
        numTabs = tabButtons.Length;
    }

    public void ShowTab(int id)
    {
        for (int i = 0; i < numTabs; ++i)
        {
            if (id == i)
            {
                tabButtons[i].interactable = false;
                panels[i].SetActive(true);
            }
            else
            {
                tabButtons[i].interactable = true;
                panels[i].SetActive(false);
            }
        }
    }

    private string GetBaseURL()
    {
        return "http://" + Settings.Host + ":" + Settings.Port;
    }

    public void RequestCountUser(string name)
    {
        StartCoroutine(RequestCountUser_Couroutine(name));
    }

    private IEnumerator RequestCountUser_Couroutine(string name)
    {
        string url = GetBaseURL() + "/";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
        }
    }
}
