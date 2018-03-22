using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

//Class to delete a user
public class DeleteUserPanel : AdminRequestPanel
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

        string url = GetBaseURL() + "/users/" + name;
        StartCoroutine(Request_Coroutine(url, "DELETE", OnSuccess, OnFailed));
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
