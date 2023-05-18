using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AI
{
    namespace Options
    {
        public class ReinforceCapitalRandom : ReinforceAction
        {

            public ReinforceCapitalRandom():base()
            {

                precondition[StateKeys.GameState] = States.Reinforce;
                precondition[StateKeys.DraftTroops] = States.Zero;

                precondition[StateKeys.Mode] = States.CapitalConquest;

                // Ends in attack state
                effects[StateKeys.GameState] = States.Draft;
                // The continent is filled as a result
                effects[StateKeys.DraftTroops] = States.Zero;

            }

            public override ActionStatus PerformAction(AIPlayer player)
            {
                int rand = RNG.Roll(0, player.PlayerRef.tiles.Count-1);

                gm.OnTileClick( player.PlayerRef.tiles[rand].ID);

                return ActionStatus.Complete;
            }



        }
    }
}