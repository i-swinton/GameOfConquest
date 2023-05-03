using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MatchSettingPair : MonoBehaviour
{

    int settingIndex;
    List<string> settingList;

    SettingType settingType;

    [SerializeField] TextMeshProUGUI settingText;
    [SerializeField] TextMeshProUGUI titleText;

    [SerializeField] Button prevButton;
    [SerializeField] Button nextButton;

   public enum SettingType
    {
        GameMode,


        AutoClaim,
        AutoReinforce,

        
    }
    public SettingType MyType
    {
        get
        {
            return settingType; 
        }
    }


    private void Awake()
    {
        settingList = new List<string>();
    }

    public void Initialize(SettingType setting)
    {
        settingType = setting;
        settingList.Clear();

        switch(settingType)
        {
            case SettingType.AutoReinforce:
                {
                    settingList.Add("False");
                    settingList.Add("True");

                    titleText.text = "Auto Reinforce";

                    break;
                }
            case SettingType.AutoClaim:
                {
                    settingList.Add("False");
                    settingList.Add("True");

                    titleText.text = "Auto Claim";
                    break;
                }
            case SettingType.GameMode:
                {
                    for(int i=0; i < GameModeList.Count; ++i)
                    {
                        // Add all of the game modes
                        settingList.Add(GameModeList.GetGameMode(i).Name);
                    }
                    titleText.text = "Game Mode";

                    break;
                }
        }

        UpdateDisplay();

    }

    

    public void Next()
    {
        ++settingIndex;

        if (settingIndex >= Count())
        {
            settingIndex = 0;
        }

        // Update the display accordingly
        UpdateDisplay();
    }
    public void Prev()
    {
        --settingIndex;

        if (settingIndex <0)
        {
            settingIndex = Count() - 1;
        }

        // Update the display accordingly
        UpdateDisplay();
    }
   
    public int GetValue()
    {
        return settingIndex;
    }
    public void SetValue(int value)
    {
        settingIndex = value;
        UpdateDisplay();
    }

    public bool GetBool()
    {
        return settingIndex == 1;
    }    
    public void SetBool(bool value)
    {
        if (value) { settingIndex = 1; }
        else { settingIndex = 0; }

        UpdateDisplay();
    }

    public void HideButtons()
    {
        prevButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
    }

    public void ShowButtons()
    {
        prevButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
    }

    // Multi-Optioned functions
    public int Count()
    {
        return settingList.Count ;
    }

    public void UpdateDisplay()
    {
        settingText.text = settingList[settingIndex];
    }
}
