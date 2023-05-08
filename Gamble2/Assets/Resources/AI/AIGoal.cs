using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public enum GoalTypes
    {

    }


    namespace Goals
    {

        [System.Serializable]
        public class AIGoal
        {
            // Get the end world goal state
            [SerializeField]
            protected WorldState worldGoal = new WorldState(0);

            public WorldState GoalState
            {
                get
                {
                    return worldGoal;
                }
            }

        }

        public class ConquerContinent : AIGoal
        {
            int targetCon;

            AIPlayer player;

            MapSystem.Board board;

            public ConquerContinent(int targetContinent, AIPlayer player)
            {
                // Int Target Continent
                targetCon = targetContinent;

                this.player = player;

                board = BoardManager.instance.GetBoard();



                // Set the target continent
                player.Blackboard.UpdateValue("TargetCon", board.FindContinent(targetCon));

                // Set up the goal list
                worldGoal[StateKeys.TargetContinent] = States.Full;

            }



        }

        public class ReinforceContinent : AIGoal
        {
            int targetCon;

            AIPlayer player;

            MapSystem.Board board;
            public ReinforceContinent(int targetContinent, AIPlayer player)
            {
                // Int Target Continent
                targetCon = targetContinent;

                this.player = player;

                board = BoardManager.instance.GetBoard();



                // Set the target continent
                player.Blackboard.UpdateValue("TargetCon", board.FindContinent(targetCon));


                // We want no draft troops
                
                worldGoal[StateKeys.DraftTroops] = States.Zero;
            }
        }

        public class TakeOverContinent : AIGoal
        {
            int targetCon;
            AIPlayer player;

            public TakeOverContinent(int targetContinent, AIPlayer player)
            {
                targetCon = targetContinent;
                this.player = player;
            }


        }
    }

}