using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotifyDisplay : MonoBehaviour
{

    //----------------------------------------------------- Variables ---------------------------------------------------------
    Queue<string> messageQueue;

    Actions.ActionList actionList;

    // The text used in the notifications
    [SerializeField]TMPro.TextMeshProUGUI textField;

    // The background containing all the text
    [SerializeField]Image notifyBG;

    //------------------------------------------------- Public Functions ------------------------------------------------------

    private void Awake()
    {
        // Set the destination to be here
        NotifySystem.Destination = this;

        // Initialize the message queue
        messageQueue = new Queue<string>();

        actionList = new Actions.ActionList();
    }

    /// <summary>
    /// Queues the message to be added to the display
    /// </summary>
    /// <param name="text"></param>
    /// <param name="duration"></param>
    public void QueueMessage(string text, float duration)
    {
        // Add to the action list
    }

    // Update is called once per frame
    void Update()
    {
        actionList.Update(Time.deltaTime);
    }
}
