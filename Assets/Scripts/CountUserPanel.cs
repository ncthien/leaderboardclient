using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

//Class to check how many times a user updated their score
public class CountUserPanel : AdminRequestPanel
{
    public Button requestButton;
    public InputField nameInput;
    public Text resultText;

    public void Request()
    {
        //Make sure that the name is ASCII only
        string name = nameInput.text;
        name = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(name));
        if (string.IsNullOrEmpty(name)) return;

        requestButton.gameObject.SetActive(false);

        string url = GetBaseURL() + "/users/" + name + "/update_count";
        StartCoroutine(Request_Coroutine(url, "GET", OnSuccess, OnFailed));
    }

    public void OnSuccess(string res)
    {
        resultText.text = res;
        requestButton.gameObject.SetActive(true);
    }

    public void OnFailed(long code, string err)
    {
        resultText.text = "ERR " + err;
        requestButton.gameObject.SetActive(true);
    }
}
