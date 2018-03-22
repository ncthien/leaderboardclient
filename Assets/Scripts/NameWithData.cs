using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Class for UI object to show a username with arbitrary data
public class NameWithData : MonoBehaviour
{
    public Text nameText;
    public Text dataText;

    public void Init(string name, string data)
    {
        nameText.text = name;
        dataText.text = data;
    }
}
