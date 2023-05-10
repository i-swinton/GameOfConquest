using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public static class AIAssist
    {

        public static States Convert(GameState state)
        {
            switch (state)
            {
                case GameState.Attack:
                    {
                        return States.Attack;
                    }
                case GameState.Claim:
                    {
                        return States.Claim;
                    }
                case GameState.Reinforce:
                    {
                        return States.Reinforce;
                    }
                case GameState.Fortify:
                    {
                        return States.Fortify;
                    }
                case GameState.End:
                    {
                        return States.End;
                    }
                case GameState.Draft:
                default:
                    {
                        return States.Draft;
                    }
            }
        }


        // Make functions
        public static AI.Goals.AIGoal MakeGoal(GoalTypes goalType, AIPlayer player)
        {
            
            switch(goalType)
            {
                case GoalTypes.ClaimRandom:
                    {
                        return new Goals.ClaimRandom(player);
                    }
                case GoalTypes.ConquerContinent:
                    {
                        //  Update the continent if needed
                        if(!player.Blackboard.Contains("TargetCon"))
                        {
                            MapSystem.Board board = BoardManager.instance.GetBoard();
                            player.Blackboard.UpdateValue("TargetCon",board.FindContinent( RNG.Roll(0, BoardManager.instance.GetBoard().ContinentCount - 1)));
                        }

                        return new Goals.ConquerContinent(
                            player.Blackboard["TargetCon"].GetContinent().ID
                            , player);
                    }
                case GoalTypes.ReinforceContinent:
                    {
                        //  Update the continent if needed
                        if (!player.Blackboard.Contains("TargetCon"))
                        {
                            MapSystem.Board board = BoardManager.instance.GetBoard();
                            player.Blackboard.UpdateValue("TargetCon", board.FindContinent(RNG.Roll(0, BoardManager.instance.GetBoard().ContinentCount - 1)));
                        }

                        return new Goals.ReinforceContinent(player.Blackboard["TargetCon"].GetContinent().ID, player);
                    }
                case GoalTypes.ReinforceRandom:
                    {
                        return new Goals.ReinforceRandom(player);
                    }
                case GoalTypes.TakeOverContinent:
                    {
                        //  Update the continent if needed
                        if (!player.Blackboard.Contains("TargetCon"))
                        {
                            MapSystem.Board board = BoardManager.instance.GetBoard();
                            player.Blackboard.UpdateValue("TargetCon", board.FindContinent(RNG.Roll(0, BoardManager.instance.GetBoard().ContinentCount - 1)));
                        }

                        return new Goals.TakeOverContinent(player.Blackboard["TargetCon"].GetContinent().ID, player);
                    }
                case GoalTypes.EndTurn:
                    {
                        return new Goals.EndTurn(player);
                    }
                default:
                    {
                        throw new System.Exception($"Goal Type {goalType.ToString()} is not listed in make goal.");
                    }
            }
        }

        public static Options.AIAction MakeAction(Options.ActionTypes actionType, AIPlayer player)
        {
            switch(actionType)
            {
                case Options.ActionTypes.AttackContinent:
                    {
                        return new Options.AttackContinent();
                    }
                case Options.ActionTypes.ClaimContinent:
                    {
                        return new Options.ClaimContinentNode();
                    }
                case Options.ActionTypes.ClaimRandom:
                    {
                        return new Options.ClaimRandom();
                    }
                case Options.ActionTypes.DraftContinent:
                    {
                        return new Options.DraftContinent(player);
                    }
                case Options.ActionTypes.DraftRandom:
                    {
                        return new Options.DraftRandom();
                    }
                case Options.ActionTypes.GoToEnd:
                    {
                        return new Options.GoToEndState();
                    }
                case Options.ActionTypes.GoToFortify:
                    {
                        return new Options.GoToFortifyState();
                    }
                case Options.ActionTypes.ReinforceContinent:
                    {
                        return new Options.ReinforceContinentNode();
                    }
                case Options.ActionTypes.ReinforceRandom:
                    {
                        return new Options.ReinforceRandom();
                    }
                default:
                    {
                        throw new System.Exception($"{actionType} could not be found in Make Action");
                    }
            }
        }

    }
}