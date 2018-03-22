using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Class for UI object to show highscore notification from server
public class NotifyMessagePanel : MonoBehaviour
{
    public Text messageText;
    public Animator animator;

	public void Startup(string name, int score, int oldScore)
    {
        string str = string.Empty;

        if (oldScore == 0)
        {
            str = "User " + name + " has set the first score of " + score.ToString();
        }
        else
        {
            str = "User " + name + " has improved his/her score to " + score.ToString();
        }

        messageText.text = str;

        //Show animation for the message
        animator.SetTrigger("Play");
    }
}
