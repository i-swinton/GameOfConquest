using MapSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Global Domination",menuName ="Game Mode/Global Domination")]
public class GlobalDomination : GameMode
{
    [Range(0,1.0f)]
    public float WinPercentage = 1;

    public override bool CheckIfOver(GameMaster master, Board board)
    {
        Player winner = CalcWinningPlayer(master, board);

        // If we pass all of these tests, then we can set the game as over
        return winner !=null;
    }

    Player CalcWinningPlayer(GameMaster master, Board board)
    {
        // Get a player to check with
        //Player player = board[0].Owner;
        

        for (int j = 0; j < master.PlayerAmount; ++j)
        {
            Player player = master.GetPlayerAt(j);

            

            int heldPositions = 0; int totalPositions =0;

            // Check all of the board tiles
            for (int i = 0; i < board.Count; ++i)
            {
                // Skip if the board is the owner
                if (board[i].Owner == null) { continue; }

               
                // If they match the owner, they own the tile
                if (player == board[i].Owner)
                {
                    ++heldPositions;
                }
                ++totalPositions;

            }

            // If they own enough positions to go over the win percentage, they are a winning player
            if(((float)heldPositions/totalPositions) >= WinPercentage )
            {
                return player;
            }

        }
        // If we made it here, no player found to be the winner
        return null;
    }

    public override Player WinningPlayer(GameMaster master, Board board)
    {
        // Retunr the turn player
        return master.GetPlayer();
    }
}
