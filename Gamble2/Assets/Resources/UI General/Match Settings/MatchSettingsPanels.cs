using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSettingsPanels : MonoBehaviour
{
    [SerializeField]
    List<MatchSettingPair> pairs;

    static MatchSettingsPanels instance;

    private void Awake()
    {
        instance = this;
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



        settings.AutoFillTiles = instance.pairs[(int)MatchSettingPair.SettingType.AutoClaim].GetBool();
        settings.AutoReinforce = instance.pairs[(int)MatchSettingPair.SettingType.AutoReinforce].GetBool();
        
        return settings;
    }

    public static GameMode GetGameMode()
    {
        // Pull the value from the game mode
        return GameModeList.GetGameMode(instance.pairs[(int)MatchSettingPair.SettingType.GameMode].GetValue());
    }

    public static int GetGameModeIndex()
    {
        return instance.pairs[(int)MatchSettingPair.SettingType.GameMode].GetValue();
    }

    public static List<bool> GetGameSettingsList()
    {
        List<bool> list = new List<bool>();

        // Fill the list with the settings on the side
        for(int i=0; i < instance.pairs.Count; ++i)
        {
            list.Add(instance.pairs[i].GetBool());
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
