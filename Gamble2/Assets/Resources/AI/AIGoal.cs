using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    [System.Serializable]
    public class AIGoal
    {
        // Get the end world goal state
        [SerializeField]
        WorldState worldGoal = new WorldState();

        public WorldState GoalState
        {
            get
            {
                return worldGoal;
            }
        }

    }
}