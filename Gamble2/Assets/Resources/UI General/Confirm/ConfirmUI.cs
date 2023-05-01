using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ConfirmUI : UIElement
{
    public enum ConfirmType
    {
        Battle, 
        Fortify,

        None =-1
    }

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

    ConfirmType current;

    int battleValue;

    static string[] battleStrings = { "Single", "Double", "Triple", "Blitz" };

    [SerializeField]
    NumberScroll confirmScroll;

    // ------------------------------------------------- Properties -----------------------------------------------------

    public static ConfirmType confirmType
    {
        get
        {
            return instance.current;
        }
    }

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

    /// <summary>
    /// Begins the confirm UI for taking various actions.
    /// </summary>
    /// <param name="text">The text to displayed at the top of the screen.</param>
    /// <param name="confirm">The type of confirmation this is.</param>
    /// <param name="tile1">The first targeted tile</param>
    /// <param name="tile2">The second targeted tile. Defaults to null.</param>
    /// <param name="value">The minimum value. Defaults to 1.</param>
    public static void BeginConfirm(string text, ConfirmType confirm, MapSystem.BoardTile tile1, MapSystem.BoardTile tile2 = null, int value =1)
    {
        //instance.IsVisible = true;

        // Set the text to desired value
        instance.confirmText.text = text;

        //float upTime = 0.5f;
        //float stayTime = 1.0f;
        float downTime = 0.5f;

        instance.actions.Add(
            new Actions.UIShow(
                instance,
                0.0f, 0.0f, Actions.ActionType.NoGroup
                ));

        instance.actions.Add(
            new Actions.Move(
                instance.endPos, instance.startPos, instance.topPanel,
                downTime, 0.0f,Actions.ActionType.NoGroup, true));
        
        // Handle the confirms for battle and fortify
        switch(confirm)
        {
            case ConfirmType.Battle:
                {
                    // Set up the battle ui for the confirm
                    instance.InitBattleScroll(tile1);
                    break;
                }
            case ConfirmType.Fortify:
                {
                    instance.InitFortifyScroll(tile1, tile2, value);
                    // Set up the fortify ui for the confirm
                    break;
                }
        }

        instance.current = confirm;
    }

    void InitFortifyScroll(MapSystem.BoardTile tile1, MapSystem.BoardTile tile2, int value)
    {
        List<string> fortifyList = new List<string>();
        // Write each i as an item in the list
        for(int i = value; i < tile1.UnitCount; ++i)
        {
            fortifyList.Add($"{i}");
        }

        // Start at the lowest number
        confirmScroll.Initialize(fortifyList, 0);
    }

    void InitBattleScroll(MapSystem.BoardTile tile)
    {
        // Get the list
        List<string> battleList = battleStrings.ToList();
        
        // Check to see how many units the child has
        if(tile.UnitCount < 4) { battleList.RemoveAt(2); }
        if(tile.UnitCount < 3) { battleList.RemoveAt(1); }

        // Initialize the scroll
        confirmScroll.Initialize(battleList, 0);
    }


    public void OnConfirm()
    {
        int value = 0;

        switch(current)
        {
            case ConfirmType.Battle:
                {
                    // Convet string to enum
                    switch(confirmScroll.PreviewString())
                    {
                        case "Single":
                            {
                                value = -1;
                                break;
                            }
                        case "Double":
                            {
                                value = -2;
                                break;
                            }
                        case "Triple":
                            {
                                value = -3;
                                break;
                            }
                        case "Blitz":
                        default:
                            {
                                value = -4;
                                break;
                            }
                    }
                    // Convert
                    //value = (int)System.Enum.Parse<Combat.CombatRollType>(confirmScroll.PreviewString());
                    break;
                }
            case ConfirmType.Fortify:
                {
                    // Conver to integer
                    value = confirmScroll.PreviewInt();
                    break;
                }
        }

        // Hide the UI
        CloseUI();

        requester.Confirm(value);
    }
    public static void CancelConfirm()
    {
        // Don't do anything if the instance is not visible
        if (!instance.IsVisible) { return; }
        instance.CloseUI();
    }

    void CloseUI()
    {
        float closeSpeed = .1f;

        // Deactivate the confirm type
        current = ConfirmType.None;

        instance.actions.Add(
            new Actions.Move
            (
                instance.startPos, instance.endPos, instance.topPanel,
                closeSpeed, 0.0f
                ));



        // Add a hide action
        actions.Add(
            new Actions.UIHide
            (this, 0, closeSpeed,Actions.ActionType.AllGroup,true)
            );
    }
}
