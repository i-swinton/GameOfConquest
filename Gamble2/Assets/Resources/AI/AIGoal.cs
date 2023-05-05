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
        States[] worldGoal = new States[(int)StateKeys.Count];

        public States[] GoalState
        {
            get
            {
                return worldGoal;
            }
        }

    }
}