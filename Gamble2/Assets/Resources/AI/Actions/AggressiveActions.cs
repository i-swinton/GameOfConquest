using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AI
{
    namespace Options
    {
        public class AttackOutward : AttackAction
        {

            public AttackOutward()
            {
                precondition[StateKeys.GameState] = States.Attack;
                precondition[StateKeys.CanAttack] = States.CanAttackAny;
               // precondition[StateKeys.TroopCount] =

                effects[StateKeys.GameState] = States.Attack;
                effects[StateKeys.AttackState] = States.HasAttacked;
                effects[StateKeys.CardGain] = States.Yes;

                hasEntered = false;
            }

            public void Reset()
            {
                attackCount = 0;
                attacker = null;
                defender = null;
                hasEntered = false;
                step = 0;
            }

            public override ActionStatus PerformAction(AIPlayer player)
            {
                // Finish if we are not in the attack phase
                if (gm.GetState() != GameState.Attack) { return ActionStatus.Complete; }

                if(!hasEntered)
                {
                    
                    hasEntered = true;
                    if (player.Blackboard.Contains("MaxNumOfAttacks"))
                    {
                        attackCount = player.Blackboard["MaxNumOfAttacks"].GetInt();
                    }
                    else { attackCount = 1; }
                }

                if (attacker == null)
                {
                    

                    // Find states nearby which player can target
                    int attackingIndex = -1;
                    int defIndex = 0;
                    int greatestDef = int.MaxValue;
                    for (int i = 0; i < player.PlayerRef.tiles.Count; ++i)
                    {
                        if (CombatSystem.CanAttack(player.PlayerRef.tiles[i]))
                        {
                            MapSystem.BoardTile tile = player.PlayerRef.tiles[i];

                            // Attack the weakest
                            for (int j = 0; j < tile.Neighbors.Count; ++j)
                            {
                                MapSystem.BoardTile def = (MapSystem.BoardTile)tile.Neighbors[j];
                                // Can't attack yourself
                                if (def.Owner.playerID == player.PlayerRef.playerID)
                                {
                                    continue;
                                }

                                if (def.UnitCount < greatestDef)
                                {
                                    defIndex = j;
                                    attackingIndex = i;
                                }
                            }
                        }
                    }


                    // If we can't find anything to attack, mark as complete
                    if (attackingIndex == -1) { Reset(); return ActionStatus.Complete; }

                    // 
                    attacker = player.PlayerRef.tiles[attackingIndex];
                    defender = (MapSystem.BoardTile)attacker.Neighbors[defIndex];
                    return ActionStatus.Working;
                }
                else
                {
                    if(PerformAttack(player) == ActionStatus.Complete)
                    {
                        return ActionStatus.Complete;
                    }
                    //switch(step)
                    //{
                    //    // Click attacker
                    //    case 0:
                    //        {
                    //            gm.OnTileClick(attacker.ID);
                    //            step = 1;
                    //            break;
                    //        }
                    //    // Click defender
                    //    case 1:
                    //        {
                    //            gm.OnTileClick(defender.ID);
                    //            step = 2;
                    //            break;
                    //        }
                    //    // Execute attack
                    //    case 2:
                    //        {
                    //            // Confirm Blitz attack
                    //            GameMaster.GetInstance().Confirm((int)ConfirmUI.BattleConfirmValue.Blitz);


                    //            // Apply has attacked
                    //            player.UpdateWorldState(StateKeys.AttackState, AI.States.HasAttacked);

                    //            // If we took the location and must
                    //            if (!gm.IsInBattle)
                    //            {
                    //                step = 0;
                    //                // Reset node for additional attacks
                    //                defender = null;
                    //                attacker = null;

                    //                attackCount--;

                    //                break;
                    //                //return ActionStatus.Complete;
                    //            }
                    //            else
                    //            {
                    //                step = 3;
                    //                break;
                    //            }
                    //        }
                    //    case 3:
                    //        {

                    //            GameMaster gm = GameMaster.GetInstance();
                    //            //MapSystem.Board board = BoardManager.instance.GetBoard();

                    //            if (gm.IsInBattle)
                    //            {
                    //                // Calculate the maximum number of troops to send

                    //                int troopsToSend = attacker.UnitCount - 1;

                    //                gm.Confirm(troopsToSend);
                    //            }
                    //            attacker = null;
                    //            defender = null;
                    //            step = 0;

                    //            if (attackCount <= 0)
                    //            {
                    //                Reset();
                    //                return ActionStatus.Complete;
                    //            }
                    //            else
                    //            {
                    //                // Attack Count--
                    //                --attackCount;
                    //            }
                    //            // return ActionStatus.Complete;
                    //            break;
                    //        }
                    //}
                }


                // If we have completed the minimum number of attacks, we are complete
                if(attackCount <=0)
                {
                    Reset();
                    return ActionStatus.Complete;
                }


                // If we cannot attack, we are complete

                return ActionStatus.Working;
            }

        }

        public class DraftOutwards : DraftAction
        {
            List<MapSystem.BoardTile> tiles = new List<MapSystem.BoardTile>();
            int step;

            public DraftOutwards():base()
            {
                precondition[StateKeys.GameState] = States.Draft;
                precondition[StateKeys.DraftTroops] = States.Nonzero;


                effects[StateKeys.GameState] = States.Attack;
                effects[StateKeys.CanAttack] = States.CanAttackAny;
                effects[StateKeys.DraftTroops] = States.Zero;

            }

            public override void Reset()
            {
                step = 0;
                hasEntered = false;

                tiles.Clear();
            }

            public override bool OnEnter()
            {
                if( base.OnEnter())
                {
                    step = 0;

                    return true;
                }

                return false;
            }

            public override ActionStatus PerformAction(AIPlayer player)
            {
                OnEnter();

                // Mark as complete if not in the current draft
                if (PerformBasicChecks(player) == ActionStatus.Complete)
                {
                    return ActionStatus.Complete;
                }

                if (tiles.Count == 0)
                {
                    // Find available draft positions
                    for (int i = 0; i < player.PlayerRef.tiles.Count; ++i)
                    {
                        MapSystem.BoardTile tile = player.PlayerRef.tiles[i];

                        // Check Neighbors
                        bool canAdd = false;
                        foreach (MapSystem.BoardTile t in tile.Neighbors)
                        {
                            // Check if both owner player id
                            if (t.Owner.playerID != tile.Owner.playerID)
                            {
                                canAdd = true;
                                break;
                            }
                        }

                        // Add the tile
                        if (canAdd)
                        {
                            tiles.Add(tile);
                        }

                    }
                    // We are unable count the tiles
                    if (tiles.Count == 0)
                    {
                        return ActionStatus.Failed;
                    }
                }
                else
                {
                    // Choose a random spot out of all of the tiles
                    int index = RNG.Roll(0, tiles.Count - 1, false);

                    // Give the selected index
                    gm.OnTileClick(tiles[index].ID);
                }

                return ActionStatus.Working;
            }
        }
    }
}
