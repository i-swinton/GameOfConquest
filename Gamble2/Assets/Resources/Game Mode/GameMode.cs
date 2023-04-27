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
}
