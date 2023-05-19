using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class SettingOption : MonoBehaviour
{
    [Header("Setting Title")]
    [SerializeField] TextMeshProUGUI Name;
    [SerializeField] TextMeshProUGUI SelectedOption;


    [SerializeField] RectTransform OptionsPannel;
    
    Setting_Pannel SettingManager;
    [SerializeField] GameObject OptionPrefab;

    [HideInInspector] public MatchSettingPair.SettingType Setting;

    public new RectTransform transform;

    int settingIndex = 0;
    List<string> options;
    string title;

    public SettingDisplayRender render;

    LayoutGroup layoutGroup;
    (float Collapsed, float Expanded) Height;
    
    // Start is called before the first frame update
    public void GenOptions(MatchSettingPair.SettingType setting)
    {
        Setting = setting;

        layoutGroup = OptionsPannel.GetComponent<LayoutGroup>();
        SettingManager = GetComponentInParent<Setting_Pannel>();

        SettingManager.AddElement(this);

        options = new List<string>();

        MatchSettingPair.GetOptions(Setting, out options, out title);

        for (int i = 0; i < options.Count; i++)
        {
            GameObject button = Instantiate(OptionPrefab, OptionsPannel.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = options[i];
            button.name = $"{title}-{options[i]}";

            int j = i;
            button.GetComponent<Button>().onClick.AddListener(delegate () { SetValue(j); });
        }

        SetTitle();

        ContentSizeFitter();

    }
    
    void ContentSizeFitter()
    {
        float contentHeight = 0;
        float lowestItem = 0;

        contentHeight += layoutGroup.padding.top + layoutGroup.padding.bottom;

        for (int i = 0; i < OptionsPannel.childCount; i++)
        {
            RectTransform trans = OptionsPannel.GetChild(i).GetComponent<RectTransform>() ;
            if (trans.anchoredPosition.y < lowestItem)
            {
                lowestItem = trans.anchoredPosition.y;
                contentHeight += trans.rect.height;
            }

        }
        OptionsPannel.sizeDelta = new Vector2(OptionsPannel.sizeDelta.x, contentHeight);

        Height = (transform.sizeDelta.y, transform.sizeDelta.y + contentHeight);
    }

    void SetTitle()
    {
        Name.text = title+":";
        SelectedOption.text = options[settingIndex];
    }

    //Called on button click, collapses other options and expands this
    public void Expand()
    {
        SettingManager.ExpandElement(this);
    }

    //Toggles option pannel on/off
    public void ShowOptions(bool value)
    {
        OptionsPannel.gameObject.SetActive(value);

        if (value)
        {
            render.Animate(SettingDisplayRender.SelectionState.Selected);
            transform.sizeDelta = new Vector2(transform.sizeDelta.x,  Height.Expanded);
        }
        else
        {
            render.Animate(SettingDisplayRender.SelectionState.OverideNone);
            transform.sizeDelta = new Vector2(transform.sizeDelta.x,Height.Collapsed);
        }

    }

    //Called by option buttons, sets option.
    public void SetValue(int value)
    {
        settingIndex = value;
        SetTitle();
        Debug.Log(value);
    }


    public int GetValue()
    {
        return settingIndex;
    }
    public bool GetBool()
    {
        return settingIndex == 1;
    }

    public void SetBase()
    {
        render.Animate(SettingDisplayRender.SelectionState.None);

    }
    public void SetHover()
    {
        render.Animate(SettingDisplayRender.SelectionState.Hover);
    }

    private void Update()
    {
        render.actionList.Update(Time.deltaTime);
    }
}

[System.Serializable]
public class SettingDisplayRender {
    public enum SelectionState
    {
        None,
        Hover,
        Selected,
        OverideNone,
    }

    public Actions.ActionList actionList=new Actions.ActionList();

    [SerializeField] Image Display;

    [SerializeField] Color BaseColor;
    [SerializeField] float Base_ChangeTime;

    [SerializeField] Color Hover_Color;
    [SerializeField] float Hover_ChangeTime;

    [SerializeField] Color Expanded_Color;
    [SerializeField] float Expanded_ChangeTime;

    SelectionState State = SelectionState.None;

    public void Animate(SelectionState state)
    {
        if (State != SelectionState.Selected || state == SelectionState.OverideNone)
        {
            actionList.Clear();

            Color startColor = Display.color;
            Color endColor = Color.magenta;
            float time = 0;

            switch (state)
            {
                case SelectionState.None:
                    endColor = BaseColor;
                    time = Base_ChangeTime;
                    break;
                case SelectionState.Hover:
                    endColor = Hover_Color;
                    time = Hover_ChangeTime;
                    break;
                case SelectionState.Selected:
                    endColor = Expanded_Color;
                    time = Expanded_ChangeTime;
                    break;
                case SelectionState.OverideNone:
                    endColor = BaseColor;
                    time = 0;
                    break;
                default:
                    Debug.LogError("WTF");
                    break;
            }
            State = state;

            actionList.Add(new Actions.ColorLerpActions(Display, endColor, time, 0));
        }
    }

}

