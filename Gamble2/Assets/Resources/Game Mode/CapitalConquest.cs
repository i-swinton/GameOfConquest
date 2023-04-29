using MapSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Capital Conquest", menuName = "Game Mode/Capital Conquest")]
public class CapitalConquest : GameMode
{
    [Range(0, 1.0f)]
    public float WinPercentage = 1;

    public List<BonusBase> CapitalBonuses;
    public override bool CheckIfOver(GameMaster master, Board board)
    {

        return CalcWinningPlayer(master, board) != null;

        //return base.CheckIfOver(master, board);
    }


    Player CalcWinningPlayer(GameMaster master, Board board)
    {
        int capitalCount = (int)((float)master.PlayerAmount * WinPercentage);
        int[] playerCounts = new int[master.PlayerAmount];
        // Look for all of the capitals
        for (int i = 0; i < board.Count; ++i)
        {
            // If a capital
            if (board[i].GetBonusOfType(BonusBase.BonusType.Capital) != null)
            {
                playerCounts[board[i].Owner.playerID]++;
                if (playerCounts[board[i].Owner.playerID] > capitalCount)
                {
                    return board[i].Owner ;
                }
            }
        }

        return null;
    }
    public override Player WinningPlayer(GameMaster master, Board board)
    {
        return CalcWinningPlayer(master, board); 
    }
}
