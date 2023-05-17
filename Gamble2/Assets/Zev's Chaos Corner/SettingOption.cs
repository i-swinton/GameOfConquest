using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SettingOption : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI Name;
    [SerializeField] GameObject OptionsPannel;
    CustomLayoutGroup LayoutGroup;
    [SerializeField] GameObject OptionPrefab;

    public MatchSettingPair.SettingType Setting;

    public new RectTransform transform;
    // Start is called before the first frame update
    public void GenOptions(MatchSettingPair.SettingType setting)
    {
        Setting = setting;

        LayoutGroup = GetComponentInParent<CustomLayoutGroup>();

        LayoutGroup.AddElement(this);

        List<string> options = new List<string>();
        string title = "";

        MatchSettingPair.GetOptions(Setting, out options, out title);

        foreach (string option in options)
        {
            GameObject button = Instantiate(OptionPrefab, OptionsPannel.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = option;
        }

        Name.text = title;

    }

    void Start()
    {
        Name.text = gameObject.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Expand()
    {
        LayoutGroup.ExpandElement(this);
    }
    public void ShowOptions(bool value)
    {
        OptionsPannel.SetActive(value);       
    }
}
