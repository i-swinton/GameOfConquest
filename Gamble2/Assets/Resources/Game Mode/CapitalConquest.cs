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
    public int CapitalUnitsAdd = 3;

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
                if (playerCounts[board[i].Owner.playerID] >= capitalCount)
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

    public override void PerformReinforcingStep(GameMaster master, Board board, BoardTile tile)
    {
        // Apply the bonuses to the tiles
        for (int i = 0; i < CapitalBonuses.Count; ++i)
        {

            tile.ApplyBonus(CapitalBonuses[i]);
        }

        // Give the tile additional troops for being capitalized
        tile.Fortify(CapitalUnitsAdd);

        // Make sure to end the turn of the player
        master.EndTurn();
    }

    public override bool ReinforcingComplete(GameMaster master, Board board)
    {
        // Check if the number of capitals are equal to the number of players

        int captialCount = 0;

        //base.PerformReinforcingStep(master, board, tile);
        for (int i = 0; i < board.Count; ++i)
        {
            if (board[i].GetBonusOfType(BonusBase.BonusType.Capital)) { captialCount++; }
            // If capitals match number of players, return true
            if (captialCount == master.PlayerAmount) { return true; }
        }

        return false;
    }

   

}
