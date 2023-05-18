using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSettingsPanels : MonoBehaviour
{
    [SerializeField]
    List<MatchSettingPair> pairs;
    
    [SerializeField]
    List<SettingOption> options;

    static MatchSettingsPanels instance;

    private void Awake()
    {
        instance = this;
    }

    public void SetOptions(List<SettingOption> values)
    {
        options = values;
    }

    private void Start()
    {
        Debug.Log("Pair Count " + pairs.Count);
        for(int i=0; i < pairs.Count; ++i)
        {
            
            // Set the setting pair based on the loop
            pairs[i].Initialize((MatchSettingPair.SettingType)i);
        }
    }

    public static GameSettings GetSettings()
    {
        GameSettings settings = new GameSettings();


        if (instance.options.Count == 0)
        {
            settings.AutoFillTiles = instance.pairs[(int)MatchSettingPair.SettingType.AutoClaim].GetBool();
            settings.AutoReinforce = instance.pairs[(int)MatchSettingPair.SettingType.AutoReinforce].GetBool();
        }
        else
        {
            settings.AutoFillTiles = instance.options[(int)MatchSettingPair.SettingType.AutoClaim].GetBool();
            settings.AutoReinforce = instance.options[(int)MatchSettingPair.SettingType.AutoReinforce].GetBool();
        }


        return settings;
    }

    public static GameMode GetGameMode()
    {
        if(instance.options.Count == 0)
        {
            return GameModeList.GetGameMode(instance.pairs[(int)MatchSettingPair.SettingType.GameMode].GetValue());

        }

        return GameModeList.GetGameMode(instance.options[(int)MatchSettingPair.SettingType.GameMode].GetValue());
        // Pull the value from the game mode
    }

    public static int GetGameModeIndex()
    {
        if (instance.options.Count == 0)
            return instance.pairs[(int)MatchSettingPair.SettingType.GameMode].GetValue();

        return instance.options[(int)MatchSettingPair.SettingType.GameMode].GetValue();

    }

    public static List<bool> GetGameSettingsList()
    {
        List<bool> list = new List<bool>();

        if (instance.options.Count == 0)
        {            // Fill the list with the settings on the side
            for (int i = 0; i < instance.pairs.Count; ++i)
            {
                list.Add(instance.pairs[i].GetBool());
            }
        }

        for (int i = 0; i < instance.options.Count; ++i)
        {
            list.Add(instance.options[i].GetBool());
        }

        // Return the list
        return list;
    }

    public static void LoadGameState(GameMode gameMode, GameSettings settings)
    {
        //
        instance.pairs[(int)MatchSettingPair.SettingType.AutoClaim].SetBool(settings.AutoFillTiles);
        instance.pairs[(int)MatchSettingPair.SettingType.AutoReinforce].SetBool(settings.AutoReinforce);

        // Set the game mode
        instance.pairs[(int)MatchSettingPair.SettingType.GameMode].SetValue(GameModeList.GetGameModeIndex(gameMode));
    }
}
