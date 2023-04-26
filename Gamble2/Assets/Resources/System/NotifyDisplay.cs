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

    [Tooltip("How far down from the top the prompt must go when displaying a message")]
    [SerializeField] float distance;

    Vector3 startPos;
    Vector3 endPos;

    //------------------------------------------------- Public Functions ------------------------------------------------------

    private void Awake()
    {
        // Set the destination to be here
        NotifySystem.Destination = this;

        // Initialize the message queue
        messageQueue = new Queue<string>();

        actionList = new Actions.ActionList();
    }
    private void Start()
    {
        startPos = notifyBG.transform.position;
        endPos = startPos - new Vector3(0, distance);
    }

    /// <summary>
    /// Queues the message to be added to the display
    /// </summary>
    /// <param name="text"></param>
    /// <param name="duration"></param>
    public void QueueMessage(string text, float duration)
    {


        // Calculate Time to
        float downTime = 0;
        float stayTime = 0;
        float upTime = 0;


        // Down
        downTime = 0.5f;

        // Stay
        stayTime = duration - 1.0f;

        // Up
        upTime = 0.5f;

        // 
        // Add to the action list
        // Move Down
        actionList.Add(
            new Actions.SetText(textField, text, .1f));
        actionList.Add(
            new Actions.Move(endPos, startPos, notifyBG.gameObject, downTime, 0, Actions.ActionType.NoGroup, false, Actions.EaseType.Linear)
            );

        // Move Up
        actionList.Add(
    new Actions.Move(startPos, endPos, notifyBG.gameObject, upTime, stayTime,
    Actions.ActionType.AllGroup, true, Actions.EaseType.Linear)
    );

    }

    // Update is called once per frame
    void Update()
    {
        actionList.Update(Time.deltaTime);
    }
}
