using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SettingOption : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Name;
    [SerializeField] GameObject OptionsPannel;
    CustomLayoutGroup LayoutGroup;

    public MatchSettingPair.SettingType Setting;

    public new RectTransform transform;
    // Start is called before the first frame update
    private void Awake()
    {
        LayoutGroup = GetComponentInParent<CustomLayoutGroup>();

        LayoutGroup.AddElement(this);

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
