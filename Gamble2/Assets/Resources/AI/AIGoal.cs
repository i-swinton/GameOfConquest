using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public enum GoalTypes
    {
        ConquerContinent,
        ReinforceContinent,
        TakeOverContinent,
        ClaimRandom,
        ReinforceRandom,
        EndTurn,

        ExpandOutward
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

                Debug.Log("Target Con: " + board.FindContinent(targetCon).Name);

                // Set the target continent
                player.Blackboard.UpdateValue("TargetCon", board.FindContinent(targetCon));

                // Set up the goal list
                worldGoal[StateKeys.TargetContinent] = States.Full;

                worldGoal[StateKeys.GameState] = States.Reinforce ;

            }

            public override string ToString()
            {
                return "Conquer Continent";
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
                worldGoal[StateKeys.GameState] = States.Draft;
                
            }

            public override string ToString()
            {
                return "Reinforce Continent";
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

                //worldGoal[StateKeys.Owns];
                worldGoal[StateKeys.GameState] = States.End;
                worldGoal[StateKeys.AttackState] = States.HasAttacked;
            }

            public override string ToString()
            {
                return "Take Over Continent";
            }
        }

        public class EndTurn : AIGoal
        {
            AIPlayer player;
            public EndTurn(AIPlayer player)
            {
                this.player = player;

                worldGoal[StateKeys.GameState] = States.End;
            }

            public override string ToString()
            {
                return "End Turn";
            }
        }

        public class ClaimRandom :AIGoal
        {
            AIPlayer player;
            public ClaimRandom(AIPlayer player)
            {
                this.player = player;

                worldGoal[StateKeys.GameState] = States.Reinforce;
            }

            public override string ToString()
            {
                return "Claim Random";
            }
        }

        public class ReinforceRandom : AIGoal
        {
            AIPlayer player;
            public ReinforceRandom(AIPlayer player)
            {
                this.player = player;
                worldGoal[StateKeys.GameState] = States.Draft;
            }

            public override string ToString()
            {
                return "Reinforce Continent";
            }
        }

        

    }

}