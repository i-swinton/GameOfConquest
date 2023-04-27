using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmUI : UIElement
{
    // ------------------------------------------------- Variables --------------------------------------------------------
    GameMaster requester;

    [SerializeField]
    TMPro.TextMeshProUGUI confirmText;

    static ConfirmUI instance;

    [SerializeField] GameObject topPanel;

    Vector2 startPos;
    Vector2 endPos;

    [Tooltip("The distance between the start and end positions")]
    [SerializeField] float endOffset;

    Actions.ActionList actions;

    // --------------------------------------------------------- Public Functions -------------------------------------------

    private void Awake()
    {
        instance = this;
        actions = new Actions.ActionList();
    }


    private void Start()
    {
        SetStartPos();

        requester = GameMaster.GetInstance();
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
        endPos = startPos + Vector2.down * endOffset;
    }


    private void Update()
    {
        actions.Update(Time.deltaTime);
    }

    public static void BeginConfirm(string text)
    {
        instance.IsVisible = true;

        // Set the text to desired value
        instance.confirmText.text = text;

        //float upTime = 0.5f;
        //float stayTime = 1.0f;
        float downTime = 0.5f;

        instance.actions.Add(
            new Actions.Move(
                instance.endPos, instance.startPos, instance.topPanel,
                downTime, 0.0f));
        

    }

    public void OnConfirm()
    {
        int value = 0;
        requester.Confirm(value);

        // Hide the UI
        CloseUI();
    }

    void CloseUI()
    {
        instance.actions.Add(
            new Actions.Move
            (
                instance.startPos, instance.endPos, instance.topPanel,
                0.5f, 0.0f
                ));



        // Add a hide action
        actions.Add(
            new Actions.UIHide
            (this, 0, 0.5f)
            );
    }
}
