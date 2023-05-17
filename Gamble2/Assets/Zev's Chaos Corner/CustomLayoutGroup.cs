using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLayoutGroup : MonoBehaviour
{
    public float Spacing = 10;
    public float Padding = 10;


    List<SettingOption> Components = new List<SettingOption>();

    [SerializeField] GameObject SettingPrefab;
    [SerializeField] GameObject OptionPrefab;

    public void GenOptions()
    {
        for (int i = 0; i < (int)MatchSettingPair. SettingType.Count; i++)
        {
            SettingOption setting = Instantiate(SettingPrefab, transform).GetComponent<SettingOption>();
            setting.Setting = (MatchSettingPair.SettingType) i;
            setting.gameObject.name = ((MatchSettingPair.SettingType)i).ToString();
            setting.transform.anchorMax = Vector2.up;
            setting.transform.anchorMin = Vector2.up;
        }
        
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

        Invoke("UpdateLayout", 0.04f);

       // UpdateLayout();
    }

    public void AddElement(SettingOption element)
    {
        Components.Add(element);

    }


    // Start is called before the first frame update
    void Start()
    {
        GenOptions();
        Invoke("UpdateLayout", 0.01f);
       //s UpdateLayout();
    }

 
}
