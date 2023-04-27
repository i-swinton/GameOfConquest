using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLossUI : UIElement
{
    [Header("Win Loss UI")]
    [Tooltip("The text mesh used for the displaying the title")]
    [SerializeField] TMPro.TextMeshProUGUI title;
    [Tooltip("The text mesh used to hold the name of the winning player")]
    [SerializeField] TMPro.TextMeshProUGUI playerNameTag;
    [SerializeField] TMPro.TextMeshProUGUI continueText;
    [SerializeField] TMPro.TextMeshProUGUI leaveText;

    Player target;

    static WinLossUI instance;

    bool isOpen;
    //------------------------------------------------------ Public Functions ---------------------------------------------------

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        
    }

    public void Leave()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Continue()
    {
        // If you are the winner, close the game
        if(GameMaster.GetInstance().Winner == target)
        {
            Leave();
        }
        // Otherwise, hide this UI
        else
        {
            IsVisible = false;
        }

        // Close the box
        isOpen = false;
    }

    public static void Open(string text, bool playerHasWon, Player player)
    {
        if (instance.isOpen) { return; } 

        instance.title.text = text;

        instance.playerNameTag.text= player.Name;

        instance.isOpen = true;

        instance.IsVisible = true;
    }

}
