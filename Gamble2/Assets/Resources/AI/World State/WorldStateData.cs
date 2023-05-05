using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{

    public enum States
    {
        // Global States
        Any,
        None,
        No,
        Yes,

        // Continental
        Continent,
        OwnContinent,


        // Game States
        Draft,
        Attack,
        Fortify,
        Claim,
        Reinforce,
        End,

        // Attack State
        CanAttackAny,
        CannotAttack,

        // TroopCount
        IncreaseTroops,


        // Territory
        IncreaseTerritory

        // Game Mode


    }

    /// <summary>
    /// The list keys to the world state arrays which AI use
    /// </summary>
    public enum StateKeys
    {
        // Game State
        GameState,
        // Game Mode
        GameMode,

        // Owns
        Owns,

        // Target
        Target,

        // Attack Data
        CanAttack,


        // Troop Counting States
        TroopCount,
        TargetTroopCount,

        // Draft Troops
        DraftTroops,

        // Count
        Count
    }

    [System.Serializable]
    public struct WorldState
    {
        // The state of the world
        WorldStateObject[] states;//= new States[(int)StateKeys.Count]; 

        public WorldState(int v = 0)
        {
            // Create the array
            states = new WorldStateObject[(int)StateKeys.Count];

            // Fill each of the slots
            states[(int)StateKeys.GameState] = States.Any;
            states[(int)StateKeys.GameMode] = States.Any;
        }

        /// <summary>
        /// Gets the state at the given key index
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public States this[StateKeys key]
        {
            get
            {
                return (States)states[(int)key];
            }
            set
            {
                if (states[(int)key] == null)
                {
                    states[(int)key] = new WorldStateObject();
                }
                states[(int)key].stateData = value;
            }
        }

        /// <summary>
        /// Gets the state at the given key index
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public States this[int key]
        {
            get
            {
                return (States)states[key];
            }
            set
            {
                if (states[(int)key] == null)
                {
                    states[(int)key] = new WorldStateObject();
                }
                states[key].stateData = value;
            }
        }

        public int GetValue(int key)
        {
            return (int)this[key];
        }

        public int GetValue(StateKeys key)
        {
            return (int)this[key];
        }

        public void SetValue(int key, int value)
        {
            states[key] = value;
        }

        public void SetValue(StateKeys key, int value)
        {
            states[(int)key] = value;
        }



    }

    public enum WorldStateType
    {
        StateEnum,
        Int,
        Pair,
        List,
    }
    public class WorldStateObject
    {
        public WorldStateType type;
        public States stateData;

        //---------------------------- Public Functions --------------------------------------------

        public static implicit operator States(WorldStateObject obj) =>obj.stateData;
        public static implicit operator int(WorldStateObject obj) =>(int)obj.stateData;

        public static implicit operator WorldStateObject(States state)
        {
            WorldStateObject obj = new WorldStateObject();
            obj.stateData = state;

            return obj;
        }

        public static implicit operator WorldStateObject(int i)
        {
            WorldStateObject obj = new WorldStateObject();
            obj.stateData = (States)i;

            return obj;
        }

        
    }

    public class WorldStateObjectPair : WorldStateObject
    {
        public StateKeys key;
        public int element1;
        public int element2;

    }
}