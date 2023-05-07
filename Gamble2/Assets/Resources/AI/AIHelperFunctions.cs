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

    }
}