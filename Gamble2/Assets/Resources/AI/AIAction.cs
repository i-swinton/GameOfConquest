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

        public int g;
        public int h;
        int f =-1;

        public AIAction prior = null;

        protected BehaviorTree tree;

        public int FCost
        {
            get
            {
                if (f == -1) { f = g + h; }

                return f;
            }
        }

        public bool IsAnyAction
        {
            get
            {
                for(int i= 0; i < WorldState.Size; ++i)
                {
                    // If I find any conditions that don't match "any", return false
                    if(precondition[i] != States.Any)
                    {
                        return false;
                    }
                }

                // If we didn't find any exceptions, then the worldstate is an any world state
                return true;
            }
        }

        
        public virtual bool Match(int index, AIAction other)
        {
            // Compare the two state values
            return effects[index] == other.precondition[index];
        }

        public static bool Match(AIAction first, AIAction second)
        {
            // 
            for(int i=0; i < WorldState.Size; ++i)
            {

                // Effects and Precondition
                //if(first.effects[i] == States.Any) { continue; }
                if(second.precondition[i] == States.Any) { continue; }

                // If they don't match
                if(!(first.Match(i, second)))
                {
                    return false;
                }

            }

            // If I do not find any exceptions 
            return true;
        }

        public static bool Match(AIAction end, WorldState goal)
        {
            for(int i=0; i < WorldState.Size; ++i)
            {
                if(end.effects[i] == States.Any) { continue; }

                // If they don't match, return
                if(!(end.Match(i, goal)))
                {
                    return false;
                }
            }
            // If we made it here, there are no exceptions
            return true;
        }

        public bool Match(int index, WorldState goal)
        {
            // Compare the two state values
            return effects[index] == goal[index];
        }

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
    //----------------------------------------- Claiming Actions ------------------------------------------------
    public class ClaimContinentNode : AIAction
    {
        public ClaimContinentNode()
        {
            precondition[StateKeys.GameState] = States.Claim;

            // Claims continent
            // Picks points around the continet
            // If filled, picks other

            precondition[StateKeys.DraftTroops] = States.Nonzero;

            // We have a target continent
            precondition[StateKeys.TargetContinent] = States.NotFull;

            // The continent is filled as a result
            effects[StateKeys.TargetContinent] = States.Empty;

        }


        public override ActionStatus PerformAction(AIPlayer player)
        {
            //return base.PerformAction(player);

            // If we have no target, just state we completed this action
            if(!player.Blackboard.Contains("TargetCon"))
            {
                return ActionStatus.Complete;
            }

            // Get the continent
            MapSystem.Continent con = player.Blackboard["TargetCon"].GetContinent();

            // Grab an open tile
            MapSystem.BoardTile target = con.GetRandomTile(null);

            // If there are non to find, return true
            if(target == null)
            {
                return ActionStatus.Complete;
            }

            var gm = GameMaster.GetInstance();

            // Claim the given tile
            gm.ClaimTiles(target);



            // Update world state

            return ActionStatus.Working;
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
            return "Actions: Fortify";
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