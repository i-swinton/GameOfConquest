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
        FogOfWar,


        Count
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

        string title; 
        GetOptions(settingType, out settingList,out title);

        titleText.text = title;

        UpdateDisplay();

    }

    public static void GetOptions(SettingType setting, out List<string> options, out string title)
    {
        List<string> optionsOut = new List<string>();
        title = null;
        switch (setting)
        {
            case SettingType.AutoReinforce:
                {
                    optionsOut.Add("False");
                    optionsOut.Add("True");

                    title = "Auto Reinforce";

                    break;
                }
            case SettingType.AutoClaim:
                {
                    optionsOut.Add("False");
                    optionsOut.Add("True");

                    title = "Auto Claim";
                    break;
                }
            case SettingType.FogOfWar:
                {
                    optionsOut.Add("False");
                    optionsOut.Add("True");

                    title = "Fog Of War";
                    break;
                }
            case SettingType.GameMode:
                {
                    for (int i = 0; i < GameModeList.Count; ++i)
                    {
                        // Add all of the game modes
                        optionsOut.Add(GameModeList.GetGameMode(i).Name);
                    }
                    title = "Game Mode";

                    break;
                }
         

        }
        
        options = optionsOut;


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
