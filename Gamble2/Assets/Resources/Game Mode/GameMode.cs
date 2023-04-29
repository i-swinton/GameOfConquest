using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameMode : ScriptableObject
{
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
}
