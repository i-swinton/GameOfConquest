using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AI
{
    namespace Goals
    {
        public class ExpandOutward : AIGoal
        {
            public ExpandOutward()
            {
                worldGoal[StateKeys.GameState] = States.End;
                worldGoal[StateKeys.AttackState] = States.HasAttacked;
                worldGoal[StateKeys.CardGain] = States.Yes;
            }

            public override string ToString()
            {
                return "Expand Outward";
            }
        }

    }
}