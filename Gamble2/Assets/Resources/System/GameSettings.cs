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

        public int ToInt()
        {
            // Write into an int
            int i = 0;
            if (ClaimTiles)
                i |= 1 << ((int)MatchSettingPair.SettingType.AutoClaim);
            if (ReinforceTiles)
                i |= 1 << ((int)MatchSettingPair.SettingType.AutoReinforce);

            return i;
        }

        public GameSettingStruct(int val)
        {
            ClaimTiles = ((1 << ((int)MatchSettingPair.SettingType.AutoClaim)) & val) != 0;
            ReinforceTiles = ((1 << ((int)MatchSettingPair.SettingType.AutoReinforce)) & val) != 0;
        }
    }
}
