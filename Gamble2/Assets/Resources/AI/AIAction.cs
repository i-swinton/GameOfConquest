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

        public enum ActionTypes
        {
            // Claim Actions
            ClaimContinent,
            ClaimRandom,
            // Reinforce Actions
            ReinforceContinent,
            ReinforceRandom,
            // Draft Actions
            DraftContinent,
            DraftRandom,
            // Attack Actions
            AttackContinent,

            // Foritfy Actions
            FortifyLastAttackReckless,

            // GoTo Actions
            GoToFortify,
            GoToEnd,


            AttackOutward,
            DraftOutwards

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
            protected GameMaster gm;
            protected MapSystem.Board board;
            protected bool hasEntered;
            public AIAction()
            {
                gm = GameMaster.GetInstance();
                board = BoardManager.instance.GetBoard();
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
                    //if (end.effects[i] == States.Any) { continue; }
                    if(goal[i] == States.Any) { continue; }

                    // If they don't match, return
                    if (!(end.Match(i, goal)))
                    {
                        return false;
                    }
                }
                // If we made it here, there are no exceptions
                return true;
            }

            public static bool Match(WorldState current, WorldState goal)
            {
                for (int i = 0; i < WorldState.Size; ++i)
                {
                    //if (end.effects[i] == States.Any) { continue; }
                    if (goal[i] == States.Any) { continue; }

                    // If they don't match, return
                    if (!(current.Get(i) == goal.Get(i)))
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
                    if (action.precondition[i] == States.Any) { continue; }

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
                return start.Get(index) == precondition.Get(index);
            }

            #endregion

            public abstract ActionStatus PerformAction(AIPlayer player);


            public virtual bool OnEnter()
            {
                if(hasEntered == true) { return false; }
                hasEntered = true;

                return true;
            }

            public void Transform(ref WorldState world)
            {
                for(int i=0; i < WorldState.Size; ++i)
                {
                    // If the effect is set to any, don't do anything to it
                    if(effects[i] == States.Any) { continue; }

                    // Otherwise change the value
                    world[i] =effects[i];
                }
            }

        }
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

                effects[StateKeys.GameState] = States.Reinforce;

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
                gm.OnTileClick(target.ID);



                // Update world state

                return ActionStatus.Working;
            }

        }

        public class ClaimRandom : AIAction
        {
            GameMaster gm;
            MapSystem.Board board;
            public ClaimRandom()
            {
                precondition[StateKeys.GameState] = States.Claim;
                precondition[StateKeys.DraftTroops] = States.Nonzero;

                effects[StateKeys.GameState] = States.Reinforce;
                effects[StateKeys.DraftTroops] = States.Nonzero;

                gm = GameMaster.GetInstance();
                board = BoardManager.instance.GetBoard();
            }

            public override ActionStatus PerformAction(AIPlayer player)
            {
                if(gm.GetState() != GameState.Claim) { return ActionStatus.Complete; }


                MapSystem.BoardTile tile = board.GetRandomTile(null);

                // If we can't find anything, try to replan
                if(tile == null) { return ActionStatus.Failed; }

                // Otherwise, claim the tile
                gm.OnTileClick(tile.ID);

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

                effects[StateKeys.GameState] = States.Draft;
            }

            public override ActionStatus PerformAction(AIPlayer player)
            {
                // This action is complete once we are no longer in the main state
                if(GameMaster.GetInstance().GetState() != GameState.Reinforce) { return ActionStatus.Complete; }

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
                gm.OnTileClick(target.ID);

                return ActionStatus.Working;
            }

        }

        public class ReinforceRandom : AIAction
        {
            GameMaster gm;

            public ReinforceRandom()
            {
                precondition[StateKeys.GameState] = States.Reinforce;
                precondition[StateKeys.DraftTroops] = States.Nonzero;

                effects[StateKeys.GameState] = States.Draft;
                effects[StateKeys.DraftTroops] = States.Zero;

                // Get the game master
                gm = GameMaster.GetInstance();
            }

            public override ActionStatus PerformAction(AIPlayer player)
            {
                if(gm.GetState() != GameState.Reinforce) { return ActionStatus.Complete; }
                //throw new System.NotImplementedException();
                

                // If the draft troops are empty, we are complete
                if(player.PlayerRef.draftTroop <= 0) { return ActionStatus.Complete; }
                // Tiles
                MapSystem.BoardTile tile = player.PlayerRef.tiles[RNG.Roll(0, player.PlayerRef.tiles.Count - 1)];
                gm.OnTileClick(tile.ID);


                // Return working otherwise
                return ActionStatus.Working;
            }
        }

        // --------------------------------------------- Draft Actions --------------------------------------------------

        public class DraftContinent : AIAction
        {
            AIPlayer targetPlayer;

            public DraftContinent(AIPlayer p)
            {
                targetPlayer = p;

                // We can draft if...
                precondition[StateKeys.GameState] = States.Draft;
                precondition[StateKeys.DraftTroops] = States.Nonzero;


                // If we have no target, just state we completed this action
                if (!targetPlayer.Blackboard.Contains("TargetCon"))
                {
                    precondition[StateKeys.ExistsIn] = States.Invalid;

                    //return ActionStatus.Complete;
                }
                else
                {
                    // Add the continent to the need to hold
                    precondition.AddValue(StateKeys.ExistsIn, targetPlayer.Blackboard["TargetCon"].GetContinent());
                }
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

                player.CardInCheck();

                // Get the continent
                MapSystem.Continent con = player.Blackboard["TargetCon"].GetContinent();

                // Grab an open tile
                MapSystem.BoardTile target = con.GetRandomTile(player.PlayerRef);

               

                // If there are non to find, replan
                if (target == null)
                {
                    return ActionStatus.Failed;
                }

                var gm = GameMaster.GetInstance();

                // Claim the given tile
                gm.DraftTile(target);

                return ActionStatus.Working;
            }
        }

        public class DraftRandom : AIAction
        {
            GameMaster gm;
            MapSystem.Board board;

            public DraftRandom()
            {
                precondition[StateKeys.GameState] = States.Draft;
                precondition[StateKeys.DraftTroops] = States.Nonzero;

                precondition[StateKeys.Owns] = States.Any;

                effects[StateKeys.GameState] = States.Attack;
                effects[StateKeys.DraftTroops] = States.Zero;

                gm = GameMaster.GetInstance();
                board = BoardManager.instance.GetBoard();
            }

            public override ActionStatus PerformAction(AIPlayer player)
            {
                if(gm.GetState() != GameState.Draft) { return ActionStatus.Complete; }

                // If all the troops are gone, we can continue
                if(player.PlayerRef.draftTroop <= 0) { return ActionStatus.Complete; }

                // See if we can card in
                player.CardInCheck();

                MapSystem.BoardTile tile = player.PlayerRef.tiles[RNG.Roll(0, player.PlayerRef.tiles.Count-1, false)];

                // Draft the random troop
                gm.OnTileClick(tile.ID);

                // Keep on working
                return ActionStatus.Working;
            }
        }

        // -------------------------------------------- Attack Actions --------------------------------------------------

        public class AttackContinent : AIAction
        {
            MapSystem.BoardTile targetAttacker;
            MapSystem.BoardTile defenderAttacker;

            MapSystem.Board board;

            int attackCount = 1;
            bool hasEntered = false;

            int step = 0;

            public AttackContinent()
            {
                precondition[StateKeys.GameState] = States.Attack;

                effects[StateKeys.GameState] = States.Attack;
                effects[StateKeys.AttackState] = States.HasAttacked;

                // Initialize the board
                board = BoardManager.instance.GetBoard();
            }

            public override ActionStatus PerformAction(AIPlayer player)
            { 
                
                // If we have no target, just state we completed this action
                if (!player.Blackboard.Contains("TargetCon"))
                {
                    return ActionStatus.Complete;
                }

                if (!hasEntered)
                {
                    hasEntered = true;
                    if (player.Blackboard.Contains("MaxNumOfAttacks"))
                    {
                        attackCount = player.Blackboard["MaxNumOfAttacks"].GetInt();
                    }
                    else { attackCount = 1; }
                    Debug.Log("Targeting: " + player.Blackboard["TargetCon"].GetContinent().Name);
                }

                // If target attacker is null, find the target attacker
                if (targetAttacker == null)
                {

                    // Get the continent
                    MapSystem.Continent con = player.Blackboard["TargetCon"].GetContinent();

                    // Grab an open tile
                    MapSystem.BoardTile target = con.GetRandomTile(player.PlayerRef);
                    int shortestLength = board.Count+1;

                    // If the target continent does not have a tile
                    if (target == null)
                    {
                        List<MapSystem.BoardTile> ts; //= new List<MapSystem.BoardTile>();
                        List<MapSystem.BoardTile> outTiles = new List<MapSystem.BoardTile>(); //= new List<MapSystem.BoardTile>();

                        // Find the closest tile
                        foreach (var t in player.PlayerRef.tiles)
                        {
                            // If you can't attack from this tile, check the next
                            if (!CombatSystem.CanAttack(t))
                            {
                                continue;
                            }
                            ts = board.FindPathTo(t, con);

                            // The out tiles path is the shorest
                            if (ts.Count < shortestLength)
                            {
                                outTiles = ts;

                                // Set the shortest length
                                shortestLength = outTiles.Count;
                            }
                           
                        }

                        // Once we have the shortest path, move in that direction
                        if(outTiles.Count > 0)
                        {
                            string s = "";
                            foreach(var t in outTiles)
                            {
                                s += $"{t}->";
                            }
                            Debug.Log(s);
                            if (outTiles[0].Owner.playerID != outTiles[1].Owner.playerID)
                            {

                                targetAttacker = outTiles[0];
                                defenderAttacker = outTiles[1];
                            }
                        }
                    }
                    else
                    {
                        // Find a spot to attack from
                        var tiles = player.PlayerRef.tiles;
                        for (int i = 0; i < player.PlayerRef.tiles.Count; ++i)
                        {
                            // See if we can attack the continent with this tile
                            if (!CombatSystem.CanAttack(tiles[i])) { continue; }

                            MapSystem.BoardTile targetable = null;

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
                                else
                                {
                                    targetable = neighbor;
                                }
                            }

                            // Set attackers which are out of range
                            if (targetable != null)
                            {
                                targetAttacker = tiles[i];
                                defenderAttacker = targetable;
                            }

                            if(targetAttacker.Owner == defenderAttacker.Owner)
                            {
                                Debug.Log("Owner Conflict Here");
                                targetAttacker = null;
                                defenderAttacker = null;
                            }

                            // If we have an attacker skip
                            if (targetAttacker != null) { break; }
                        }
                    }

                    // If the target is still not found, report this as true
                    if (targetAttacker == null)
                    {
                        if (attackCount <= 0)
                        {
                            hasEntered = false;
                            return ActionStatus.Complete;
                        }
                        else
                        {
                            // Attack Count--
                            --attackCount;
                        }
                    }

                }
                else
                {
                    switch (step)
                    {
                        case 0:
                            {
                                //GameMaster.GetInstance().AttackTile(GenerateMap.GetTile(targetAttacker));
                                GameMaster.GetInstance().OnTileClick(targetAttacker.ID);
                                if (GameMaster.GetInstance().HasChallengerCheck())
                                {
                                    step = 1;
                                }
                                break;
                            }
                        case 1:
                            {

                                //GameMaster.GetInstance().AttackTile(GenerateMap.GetTile(defenderAttacker));
                                GameMaster.GetInstance().OnTileClick(defenderAttacker.ID);
                                if (GameMaster.GetInstance().HasDefenderCheck())
                                {
                                    step = 2;
                                }
                                break;
                            }
                        case 2:
                            {
                                Debug.Log($"{targetAttacker.Name} is attacking {defenderAttacker.Name} ");
                                // Confirm Blitz attack
                                GameMaster.GetInstance().Confirm((int)ConfirmUI.BattleConfirmValue.Blitz);


                                // Apply has attacked
                                player.UpdateWorldState(StateKeys.AttackState, AI.States.HasAttacked);

                                // If we took the location and must
                                if (!GameMaster.GetInstance().IsInBattle)
                                {
                                    step = 0;
                                    // Reset node for additional attacks
                                    targetAttacker = null;
                                    defenderAttacker = null;

                                    if (attackCount <= 0)
                                    {
                                        hasEntered = false;
                                        return ActionStatus.Complete;
                                    }
                                    else
                                    {
                                        // Attack Count--
                                        --attackCount;
                                    }
                                    break;
                                    //return ActionStatus.Complete;
                                }
                                else
                                {
                                    step = 3;
                                    break;
                                }
                            }
                        case 3:
                            {

                                GameMaster gm = GameMaster.GetInstance();
                                //MapSystem.Board board = BoardManager.instance.GetBoard();

                                if (gm.IsInBattle)
                                {
                                    // Calculate the maximum number of troops to send

                                    int troopsToSend = targetAttacker.UnitCount - 1;

                                    gm.Confirm(troopsToSend);
                                }
                                targetAttacker = null;
                                defenderAttacker = null;
                                step = 0;

                                if (attackCount <= 0)
                                {
                                    hasEntered = false;
                                    return ActionStatus.Complete;
                                }
                                else
                                {
                                    // Attack Count--
                                    --attackCount;
                                }
                                // return ActionStatus.Complete;
                                break;
                            }
                    }
                }

                return ActionStatus.Working;
            }
        }

       

        // -------------------------------------------- Fortify Actions -------------------------------------------------

        public class FortifyLastAttackReckless : AIAction
        {
            MapSystem.BoardTile targetTile;
            MapSystem.BoardTile defenderTile;
            MapSystem.Board board;
            int step;
            GameMaster gm;
            public FortifyLastAttackReckless()
            {
                precondition[StateKeys.GameState] = States.Fortify;
                effects[StateKeys.GameState] = States.End;

                // We need to have attacked to take this action
                precondition[StateKeys.AttackState] = States.HasAttacked;
                //precondition[StateKeys.GameState] = States.Fortify;

                // Ends the fortify state
                //effects[StateKeys.GameState] = States.End;

                // Grab the board
                board = BoardManager.instance.GetBoard();
                gm = GameMaster.GetInstance();
            }

            public override ActionStatus PerformAction(AIPlayer player)
            {
                // Step 1, Get the target tile
                if (targetTile == null)
                {
                    if (player.Blackboard.Contains("LastAttackTileID"))
                    {
                        // If we didn't attack for any reason, stop and reset
                        if (player.Blackboard["LastAttackTileID"].GetInt() == -1)
                        {
                            gm.ForceTurnEnd();
                            step = 0;
                            targetTile = null;
                            defenderTile = null;
                            return ActionStatus.Complete;
                        }
                        else
                        {


                            targetTile = board[player.Blackboard["LastAttackTileID"].GetInt()];
                            if (targetTile.Owner.playerID != player.PlayerRef.playerID)
                            {
                                Debug.Log("Mismatch detected");
                                Debug.Break();
                            }
                        }
                    }
                    else
                    {
                        // Report failure if we have not attacked
                        return ActionStatus.Failed;
                    }
                }
                else if (defenderTile == null)
                {
                    // Step 2, Pick anothter tile
                    List<MapSystem.BoardTile> connectedTiles = board.GetConnectedTiles(targetTile.ID);

                    // Find the highest cost 
                    MapSystem.BoardTile highestTile=null;
                    int highest = 1;
                    for(int i=0; i < connectedTiles.Count; ++i)
                    {
                        // Check if it is the new highest
                        if(connectedTiles[i].UnitCount > highest)
                        {
                            // Update the highest tile
                            highestTile = connectedTiles[i];
                            highest = highestTile.UnitCount;
                        }
                    }
                    // If we could not find a higher tile, just end the foritfy
                    if(highestTile == null || highestTile == targetTile)
                    {
                        targetTile = null;

                        // End the turn
                        GameMaster.GetInstance().ForceTurnEnd();

                        return ActionStatus.Complete;
                    }
                    // Else, lets prepare the fortify
                    defenderTile = highestTile;
                }
                else
                {
                    Debug.Log($"Target Tile: {targetTile}, Defender Tile: {defenderTile}");
                    // Step 3, Profit
                    switch(step)
                    {
                        // Step 3.1: Target Challenger
                        case 0:
                            {
                                gm.OnTileClick(defenderTile.ID);
                                step = 1;
                                break;
                            }
                        //Step 3.2: Target Defender
                        case 1:
                            {
                                gm.OnTileClick(targetTile.ID);
                                step = 2;
                                break;
                            }
                        // Step 3.3 Do it
                        case 2:
                            {
                                step = 0;

                                // Confirm
                                gm.Confirm(defenderTile.UnitCount - 1);

                                // Target Tile
                                targetTile = null;
                                defenderTile = null;

                                // Return complete
                                return ActionStatus.Complete;
                            }
                    }
                }

                // Make sure to shut down everything in step 3


                return ActionStatus.Working;
            }

            public override string ToString()
            {
                return "Fortify Last Attack Reckless"; 
            }
        }

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
                return "Go To Fortify";
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

        // ----------------------------------- Other Action --------------------------------------------

       
        // ------------------------------------ Template Actions --------------------------------------
        public abstract class AttackAction : AIAction
        {
            protected MapSystem.BoardTile attacker;
            protected MapSystem.BoardTile defender;
        }

    }
}