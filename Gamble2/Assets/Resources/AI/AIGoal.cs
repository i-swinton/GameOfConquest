using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public enum GoalTypes
    {

    }


    [System.Serializable]
    public class AIGoal
    {
        // Get the end world goal state
        [SerializeField]
        WorldState worldGoal = new WorldState(0);

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
        }
    }



}