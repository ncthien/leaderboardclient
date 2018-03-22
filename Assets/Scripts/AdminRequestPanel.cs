using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//Base class for admin web request
public class AdminRequestPanel : MonoBehaviour
{
    protected string GetBaseURL()
    {
        return "http://" + Settings.Host + ":" + Settings.Port;
    }

    //Send request and wait for reply from server
    protected IEnumerator Request_Coroutine(string url, string method, Action<string> onSuccess, Action<long, string> onFailed)
    {
        using (UnityWebRequest www = new UnityWebRequest(url, method, new DownloadHandlerBuffer(), null))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                onFailed(www.responseCode, www.error);
            }
            else
            {
                onSuccess(www.downloadHandler.text);
            }
        }
    }
}
