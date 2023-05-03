using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameMode : ScriptableObject
{
    [SerializeField] string modeName;

    public string Name
    {
        get
        {
            return modeName;
        }
    }

    public virtual bool CheckIfOver(GameMaster master, MapSystem.Board board)
    {
        return false;
    }

    public virtual Player WinningPlayer(GameMaster master, MapSystem.Board board)
    {
        return master.GetPlayer();
    }
   

    public virtual bool ReinforcingComplete(GameMaster master, MapSystem.Board board)
    {
        return true;
    }

    public virtual void PerformReinforcingStep(GameMaster master, MapSystem.Board board, MapSystem.BoardTile tile)
    {
        return;
    }

    public virtual bool OverrideReinforcement()
    {
        return false;
    }

    [Tooltip("The number of tiles per each extra troop a player can get")]
    [SerializeField] int tilesPerTroop = 3;

    public virtual int TilesPerTroop
    {
        get
        {
            if(tilesPerTroop < 1) { return 1; }
            return tilesPerTroop;
        }
    }

}
