using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSettings 
{
    public bool AutoFillTiles = false;
    public bool AutoReinforce = false;

    public GameSettings()
    {
        Debug.Log("Writing game settings");
    }

    public GameSettings(List<bool> settings)
    {
        AutoFillTiles = settings[(int)MatchSettingPair.SettingType.AutoClaim];
        AutoReinforce = settings[(int)MatchSettingPair.SettingType.AutoReinforce] ;
    }

}
