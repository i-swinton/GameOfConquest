using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    namespace Options
    {
        public enum ActionStatus
        {
            Complete,
            Working,
            Failed
        }


        public abstract class AIAction
        {

            protected WorldState precondition = new WorldState(0);
            protected WorldState effects = new WorldState(0);

            // The weight of the action
            protected int weight = 1;

            public int g;
            public int h;
            int f = -1;

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
                    for (int i = 0; i < WorldState.Size; ++i)
                    {
                        // If I find any conditions that don't match "any", return false
                        if (precondition[i] != States.Any)
                        {
                            return false;
                        }
                    }

                    // If we didn't find any exceptions, then the worldstate is an any world state
                    return true;
                }
            }

            #region Match Functions
            public virtual bool Match(int index, AIAction other)
            {
                // Compare the two state values
                return effects[index] == other.precondition[index];
            }

            public static bool Match(AIAction first, AIAction second)
            {
                // 
                for (int i = 0; i < WorldState.Size; ++i)
                {

                    // Effects and Precondition
                    //if(first.effects[i] == States.Any) { continue; }
                    if (second.precondition[i] == States.Any) { continue; }

                    // If they don't match
                    if (!(first.Match(i, second)))
                    {
                        return false;
                    }

                }

                // If I do not find any exceptions 
                return true;
            }

            public static bool Match(AIAction end, WorldState goal)
            {
                for (int i = 0; i < WorldState.Size; ++i)
                {
                    if (end.effects[i] == States.Any) { continue; }

                    // If they don't match, return
                    if (!(end.Match(i, goal)))
                    {
                        return false;
                    }
                }
                // If we made it here, there are no exceptions
                return true;
            }

            public static bool Match(WorldState start, AIAction action)
            {
                for (int i = 0; i < WorldState.Size; ++i)
                {
                    if (start[i] == States.Any) { continue; }

                    // If they don't match, return
                    if (!(action.MatchPre(i, start)))
                    {
                        return false;
                    }
                }
                // If we made it here, there are no exceptions
                return true;
            }

            //public static bool Match(WorldState state1)

            public bool Match(int index, WorldState goal)
            {
                // Compare the two state values
                return effects.Get(index) == goal.Get(index);
            }
            public bool MatchPre(int index, WorldState start)
            {
                return precondition.Get(index) == start.Get(index);
            }

            #endregion

            public abstract ActionStatus PerformAction(AIPlayer player);

        }



        //public class AttackTarget : AIAction
        //{
        //    public AttackTarget()
        //    {
        //        // Adjust the world state
        //        precondition[StateKeys.GameState] = States.Attack;

        //        //

        //    }
        //}

        //public class AttackAdjacent : AIAction
        //{
        //    public AttackAdjacent()
        //    {

        //    }
        //}

        //public class AttackSameContinent : AIAction
        //{
        //    public AttackSameContinent()
        //    {

        //    }
        //}
        //----------------------------------------- Claiming Actions ------------------------------------------------
        public class ClaimContinentNode : AIAction
        {
            public ClaimContinentNode()
            {
                precondition = new WorldState(0);
                effects = new WorldState(0);

                precondition[StateKeys.GameState] = States.Claim;

                // Claims continent
                // Picks points around the continet
                // If filled, picks other

                precondition[StateKeys.DraftTroops] = States.Nonzero;

                // We have a target continent
                precondition[StateKeys.TargetContinent] = States.NotFull;

                // The continent is filled as a result
                effects[StateKeys.TargetContinent] = States.Full;

            }


            public override ActionStatus PerformAction(AIPlayer player)
            {
                //return base.PerformAction(player);

                // If we have no target, just state we completed this action
                if (!player.Blackboard.Contains("TargetCon"))
                {
                    return ActionStatus.Complete;
                }

                // Get the continent
                MapSystem.Continent con = player.Blackboard["TargetCon"].GetContinent();

                // Grab an open tile
                MapSystem.BoardTile target = con.GetRandomTile(null);

                // If there are non to find, return true
                if (target == null)
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

        //---------------------------------------------- Reinforce Actions -----------------------------------------------
        public class ReinforceContinentNode : AIAction
        {
            public ReinforceContinentNode()
            {
                precondition[StateKeys.GameState] = States.Reinforce;

                // Claims continent
                // Picks points around the continet
                // If filled, picks other

                precondition[StateKeys.DraftTroops] = States.Nonzero;

                // We have a target continent
                precondition[StateKeys.TargetContinent] = States.Full;

                // Unit owns a location on that continent
                //precondition[stat]

                // The continent is filled as a result
                effects[StateKeys.DraftTroops] = States.Zero;
            }

            public override ActionStatus PerformAction(AIPlayer player)
            {
                // If we have no target, just state we completed this action
                if (!player.Blackboard.Contains("TargetCon"))
                {
                    return ActionStatus.Complete;
                }
                // We are done once we are out of draft troops
                if(player.PlayerRef.draftTroop <=0)
                {
                    return ActionStatus.Complete;
                }

                // Get the continent
                MapSystem.Continent con = player.Blackboard["TargetCon"].GetContinent();

                // Grab an open tile
                MapSystem.BoardTile target = con.GetRandomTile(player.PlayerRef);


                // If there are non to find, return true
                if (target == null)
                {
                    return ActionStatus.Complete;
                }

                var gm = GameMaster.GetInstance();

                // Claim the given tile
                gm.ReinforceTile(target);

                return ActionStatus.Working;
            }

        }

        // --------------------------------------------- Draft Actions --------------------------------------------------

        public class DraftContinent : AIAction
        {
            public DraftContinent()
            {
                // We can draft if...
                precondition[StateKeys.GameState] = States.Draft;
                precondition[StateKeys.DraftTroops] = States.Nonzero;
                //precondition[StateKeys]

                // Becuase we draft continents...
                effects[StateKeys.GameState] = States.Attack;
                effects[StateKeys.DraftTroops] = States.Zero;
            }

            public override ActionStatus PerformAction(AIPlayer player)
            {


                // If we have no target, just state we completed this action
                if (!player.Blackboard.Contains("TargetCon"))
                {
                    return ActionStatus.Complete;
                }
                // We are done once we are out of draft troops
                if (player.PlayerRef.draftTroop <= 0)
                {
                    return ActionStatus.Complete;
                }

                // Get the continent
                MapSystem.Continent con = player.Blackboard["TargetCon"].GetContinent();

                // Grab an open tile
                MapSystem.BoardTile target = con.GetRandomTile(player.PlayerRef);


                // If there are non to find, return true
                if (target == null)
                {
                    return ActionStatus.Complete;
                }

                var gm = GameMaster.GetInstance();

                // Claim the given tile
                gm.DraftTile(target);

                return ActionStatus.Working;
            }
        }

        // -------------------------------------------- Attack Actions --------------------------------------------------

        public class AttackContinent : AIAction
        {
            MapSystem.BoardTile targetAttacker;
            MapSystem.BoardTile defenderAttacker;

            int step = 0;

            public AttackContinent()
            {

            }

            public override ActionStatus PerformAction(AIPlayer player)
            { 
                
                // If we have no target, just state we completed this action
                if (!player.Blackboard.Contains("TargetCon"))
                {
                    return ActionStatus.Complete;
                }

                // If target attacker is null, find the target attacker
                if (targetAttacker == null)
                {

                    // Get the continent
                    MapSystem.Continent con = player.Blackboard["TargetCon"].GetContinent();

                    // Grab an open tile
                    MapSystem.BoardTile target = con.GetRandomTile(player.PlayerRef);



                    var tiles = player.PlayerRef.tiles;
                    for (int i = 0; i < player.PlayerRef.tiles.Count; ++i)
                    {
                        // See if we can attack the continent with this tile
                        if (!CombatSystem.CanAttack(tiles[i])) { continue; }

                        // Search through the neighbors for tiles in the continent
                        foreach (MapSystem.BoardTile neighbor in tiles[i].Neighbors)
                        {
                            // If I cannot attack my neighbors, continue
                            if (neighbor.Owner == null || neighbor.Owner == player.PlayerRef)
                            {
                                continue;
                            }

                            // If I neighbor a tile and I can attack it
                            if (con.Contains(neighbor))
                            {
                                targetAttacker = tiles[i];
                                defenderAttacker = neighbor;
                                break;
                            }
                        }

                        // If we have an attacker skip
                        if (targetAttacker != null) { break; }
                    }

                }
                else
                {
                    switch (step)
                    {
                        case 0:
                            {
                                GameMaster.GetInstance().AttackTile(GenerateMap.GetTile(targetAttacker));
                                step = 1;
                                break;
                            }
                        case 1:
                            {

                                GameMaster.GetInstance().AttackTile(GenerateMap.GetTile(defenderAttacker));
                                step = 2;
                                break;
                            }
                        case 2:
                            {
                                // Confirm Blitz attack
                                GameMaster.GetInstance().Confirm((int)ConfirmUI.BattleConfirmValue.Blitz);
                                // Reset node for additional attacks
                                targetAttacker = null;
                                defenderAttacker = null;
                                step = 0;


                                return ActionStatus.Complete;
                                
                            }
                    }
                }



                return ActionStatus.Working;
            }
        }

        // -------------------------------------------- Fortify Actions -------------------------------------------------

        // -------------------------------------------- End Turn Actions ------------------------------------------------

        // ----------------------------------------- State Change Actions -------------------------------------------

        public class GoToAttackState : AIAction
        {
            public GoToAttackState()
            {
                precondition[StateKeys.GameState] = States.Draft;
                precondition[StateKeys.DraftTroops] = States.None;

                effects[StateKeys.GameState] = States.Attack;
            }

            public override ActionStatus PerformAction(AIPlayer player)
            {

                // End the turn
                GameMaster.GetInstance().ForceTurnEnd();

                return ActionStatus.Complete;
                //throw new System.NotImplementedException();
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

            public override ActionStatus PerformAction(AIPlayer player)
            {

                // End the turn
                GameMaster.GetInstance().ForceTurnEnd();

                return ActionStatus.Complete;
                //throw new System.NotImplementedException();
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


            public override ActionStatus PerformAction(AIPlayer player)
            {

                // End the turn
                GameMaster.GetInstance().ForceTurnEnd();

                return ActionStatus.Complete;
                //throw new System.NotImplementedException();
            }
        }


    }
}