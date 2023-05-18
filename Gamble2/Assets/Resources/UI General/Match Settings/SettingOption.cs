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


    [SerializeField] GameObject OptionsPannel;

    Setting_Pannel LayoutGroup;
    [SerializeField] GameObject OptionPrefab;

    [HideInInspector] public MatchSettingPair.SettingType Setting;

    public new RectTransform transform;

    int settingIndex = 0;
    List<string> options;
    string title;

    (float Collapsed, float Expanded) Height;
    
    // Start is called before the first frame update
    public void GenOptions(MatchSettingPair.SettingType setting)
    {
        Setting = setting;

        LayoutGroup = GetComponentInParent<Setting_Pannel>();

        LayoutGroup.AddElement(this);

        options = new List<string>();

        MatchSettingPair.GetOptions(Setting, out options, out title);

        for (int i = 0; i < options.Count; i++)
        {
            GameObject button = Instantiate(OptionPrefab, OptionsPannel.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = options[i] + $"{i}";
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
        RectTransform item = OptionsPannel.GetComponent<RectTransform>();
        for (int i = 0; i < item.childCount; i++)
        {
            RectTransform trans = item.GetChild(i).GetComponent<RectTransform>() ;
            if (trans.anchoredPosition.y < lowestItem)
            {
                lowestItem = trans.anchoredPosition.y;
                contentHeight += trans.rect.height;
            }

        }
        item.GetComponent<RectTransform>().sizeDelta = new Vector2(item.sizeDelta.x, contentHeight);

        Height = (transform.sizeDelta.y, transform.sizeDelta.y + contentHeight);
    }

    void SetTitle()
    {
        Name.text = title+":";
        SelectedOption.text = options[settingIndex];
        //$"{title} ({options[settingIndex]}";
    }


    public void Expand()
    {
        LayoutGroup.ExpandElement(this);
    }
    public void ShowOptions(bool value)
    {
        OptionsPannel.SetActive(value);

        transform.sizeDelta = new Vector2(transform.sizeDelta.x, value ? Height.Expanded:Height.Collapsed);
    }

    public void SetValue(int value)
    {
        SetTitle();
        settingIndex = value;
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

}
