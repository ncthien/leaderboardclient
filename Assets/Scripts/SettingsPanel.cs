using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Class for UI component to setup server settings
public class SettingsPanel : MonoBehaviour
{
    public InputField hostInput;
    public InputField portInput;
    public InputField portWSInput;

    public GameObject fadePanel;
    public GameObject inputPanel;

    private bool isShown;

    private void Start()
    {
        //Load server host from cached (if exists)
        if (PlayerPrefs.HasKey("host"))
        {
            Settings.Host = PlayerPrefs.GetString("host");
        }
        else
        {
            PlayerPrefs.SetString("host", Settings.Host);
        }

        //Load server port from cached (if exists)
        if (PlayerPrefs.HasKey("port"))
        {
            Settings.Port = PlayerPrefs.GetInt("port");
        }
        else
        {
            PlayerPrefs.SetInt("port", Settings.Port);
        }

        //Load server websocket port from cached (if exists)
        if (PlayerPrefs.HasKey("port_ws"))
        {
            Settings.PortWS = PlayerPrefs.GetInt("port_ws");
        }
        else
        {
            PlayerPrefs.SetInt("port_ws", Settings.PortWS);
        }

        hostInput.text = Settings.Host;
        portInput.text = Settings.Port.ToString();
        portWSInput.text = Settings.PortWS.ToString();

        isShown = false;
    }

    //Show/hide the settings panel
    public void TogglePanel()
    {
        isShown = !isShown;

        if (isShown)
        {
            fadePanel.SetActive(true);
            inputPanel.SetActive(true);
        }
        else
        {
            fadePanel.SetActive(false);
            inputPanel.SetActive(false);

            PlayerPrefs.Save();
        }
    }

    //Update new server host
    public void OnChangeHost()
    {
        Settings.Host = hostInput.text;
        PlayerPrefs.SetString("host", Settings.Host);
    }

    //Update new server port
    public void OnChangePort()
    {
        int port;
        if (int.TryParse(portInput.text, out port))
        {
            if (port > 0)
            {
                Settings.Port = port;
                PlayerPrefs.SetInt("port", Settings.Port);
            }
        }
    }

    //Update new server websocket port
    public void OnChangePortWS()
    {
        int portWS;
        if (int.TryParse(portWSInput.text, out portWS))
        {
            Settings.PortWS = portWS;
            PlayerPrefs.SetInt("port_ws", Settings.PortWS);
        }
    }
}
