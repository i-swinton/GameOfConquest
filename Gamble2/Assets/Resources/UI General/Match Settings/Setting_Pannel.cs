using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Setting_Pannel : MonoBehaviour
{
    public float Spacing = 10;
    public float Padding = 10;


    List<SettingOption> Components = new List<SettingOption>();

    [SerializeField] GameObject SettingPrefab;
    [SerializeField] MatchSettingsPanels Setter;
    public void GenOptions()
    {
        
        for (int i = 0; i < (int)MatchSettingPair. SettingType.Count; i++)
        {
            SettingOption setting = Instantiate(SettingPrefab, transform).GetComponent<SettingOption>();
            setting.GenOptions((MatchSettingPair.SettingType) i);
            setting.gameObject.name = ((MatchSettingPair.SettingType)i).ToString();
            setting.transform.anchorMax = Vector2.up;
            setting.transform.anchorMin = Vector2.up;

        }
        Setter.SetOptions(Components);
    }

    public void UpdateLayout()
    {
        float lowest = Padding;
   //     lowest += Components[0].transform.rect.height/2;
        foreach (SettingOption item in Components)
        {
            lowest += item.transform.rect.height / 2;
            item.transform.anchoredPosition3D = new Vector3(item.transform.anchoredPosition3D.x, -lowest,item.transform.anchoredPosition3D.z);

            lowest += Spacing;
            lowest += item.transform.rect.height / 2;
        }
    }

    public void ExpandElement(SettingOption element)
    {
        foreach (SettingOption item in Components)
        {
            item.ShowOptions(false);
        }
        element.ShowOptions(true);

        UpdateLayout();
    }

    public void AddElement(SettingOption element)
    {
        Components.Add(element);

    }


    // Start is called before the first frame update
    void Start()
    {
        GenOptions();

        UpdateLayout();
    }


}
