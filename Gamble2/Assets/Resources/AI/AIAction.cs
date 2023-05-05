using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public enum ActionStatus
    {
        Complete,
        Working,
        Failed
    }


    public class AIAction
    {

        protected WorldState precondition;
        protected WorldState effects;

        // The weight of the action
        protected int weight = 1;

        public virtual ActionStatus PerformAction(AIPlayer player)
        {
            // If not overridden, then just claim the action is done
            return ActionStatus.Complete;
        }

    }

    

    public class AttackTarget : AIAction
    {
        public AttackTarget()
        {
            // Adjust the world state
            precondition[StateKeys.GameState] = States.Attack;

            //
            //effects[StateKeys]
        }
    }

    public class AttackAdjacent : AIAction
    {
        public AttackAdjacent()
        {

        }
    }
        
    public class AttackSameContinent :AIAction
    {
        public AttackSameContinent()
        {

        }
    }

 // ----------------------------------------- State Change Actions -------------------------------------------

    public class GoToAttackState : AIAction
    {
        public GoToAttackState()
        {
            precondition[StateKeys.GameState] = States.Draft;
            precondition[StateKeys.DraftTroops] = States.None;

            effects[StateKeys.GameState] = States.Attack;
        }
    }

    public class GoToFortifyState : AIAction
    {
        public GoToFortifyState()
        {
            precondition[StateKeys.GameState] = States.Attack;
            effects[StateKeys.GameState] = States.Fortify;
        }

        public override string ToString()
        {
            return "Actrions: Fortify";
        }
    }

    public class GoToEndState : AIAction
    {
        public GoToEndState()
        {
            precondition[StateKeys.GameState] = States.Fortify;
            effects[StateKeys.GameState] = States.End;
        }

        public override string ToString()
        {
            return "Action: End";
        }
    }

    
}