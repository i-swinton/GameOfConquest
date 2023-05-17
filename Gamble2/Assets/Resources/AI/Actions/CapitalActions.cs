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

            }

            public override ActionStatus PerformAction(AIPlayer player)
            {
                int rand = RNG.Roll(0, player.PlayerRef.tiles.Count);

                gm.OnTileClick( player.PlayerRef.tiles[rand].ID);

                return ActionStatus.Complete;
            }



        }
    }
}