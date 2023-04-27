using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmUI : UIElement
{

    GameMaster requester;

    TMPro.TextMeshProUGUI confirmText;

    static ConfirmUI instance;

    [SerializeField] GameObject topPanel;

    Vector2 startPos;
    Vector2 endPos;

    [Tooltip("The distance between the start and end positions")]
    [SerializeField] float endOffset;

    Actions.ActionList actions;

    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        SetStartPos();
    }

    private void OnValidate()
    {
        if(topPanel !=null && Application.isPlaying)
        {
            SetStartPos();
        }
    }

    void SetStartPos()
    {
        startPos = topPanel.transform.position;
        endPos = startPos - Vector2.down * endOffset;
    }


    private void Update()
    {
        actions.Update(Time.deltaTime);
    }

    public static void BeginConfirm(string text)
    {
        // Set the text to desired value
        instance.confirmText.text = text;
    }

    public void OnConfirm()
    {
        int value = 0;
        requester.Confirm(value);
    }
}
