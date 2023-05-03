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
    public GameSettings(NetworkSystem.GameSettingStruct gss)
    {
        AutoFillTiles = gss.ClaimTiles;
        AutoReinforce = gss.ReinforceTiles;
    }
}


namespace NetworkSystem
{
    [System.Serializable]
    public struct GameSettingStruct
    {
        public bool ClaimTiles;
        public bool ReinforceTiles;

        //public GameSettingStruct(List<bool> settings)
        //{
        //    ClaimTiles = false;
        //    ReinforceTiles = false;
        //}

        public GameSettingStruct(List<bool> settings)
        {
            ClaimTiles = settings[(int)MatchSettingPair.SettingType.AutoClaim];
            ReinforceTiles = settings[(int)MatchSettingPair.SettingType.AutoReinforce];
        }
        public GameSettingStruct(GameSettingStruct other)
        {
            ClaimTiles = other.ClaimTiles;
            ReinforceTiles = other.ReinforceTiles;
        }
    }
}
