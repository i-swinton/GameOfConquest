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

        Zero,
        Nonzero,

        Empty,
        NotFull,
        Full,

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
        IncreaseTerritory,

        // Game Mode

        // Blackboard
        Home,

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
        OwnsPartially,

        // Target
        TargetPlayer,
        TargetContinent,

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

        public static int Size
        {
            get
            {
                return (int)StateKeys.Count;
            }
        }

        public WorldState(int v = 0)
        {
            // Create the array
            states = new WorldStateObject[(int)StateKeys.Count];

            // Fill each of the slots
            states[(int)StateKeys.GameState] = States.Any;
            states[(int)StateKeys.GameMode] = States.Any;

            states[(int)StateKeys.DraftTroops] = States.Any;

            states[(int)StateKeys.CanAttack] = States.Any;
            states[(int)StateKeys.Owns] = States.Any;
            states[(int)StateKeys.OwnsPartially] = States.Any;
            states[(int)StateKeys.TargetContinent] = States.Any;
            states[(int)StateKeys.TargetTroopCount] = States.Any;
            states[(int)StateKeys.TroopCount] = States.Any;
            //states[(int)StateKeys.] = States.Any;

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
                if (states[(int)key] == null)
                {
                    states[(int)key] = new WorldStateObject();
                }
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

        public WorldStateObject Get(StateKeys key)
        {
            return states[(int)key];
        }
        public WorldStateObject Get(int index)
        {
            return states[index];
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

        public static bool operator==(WorldStateObject x, WorldStateObject y)
        {
            if (x is null) { return y is null; }
            else if (y is null) { return x is null; }
            // Since it is state data
            if(y.stateData == States.Any) { return true; }

            // Then just match the x/y
            return x.stateData == y.stateData;
        }

        public static bool operator!=(WorldStateObject x, WorldStateObject y)
        {
            if (x is null) { return !(y is null); }
            else if (y is null) { return !(x is null); }

            // IF the second is any, than the two match
            if (y.stateData == States.Any) { return false; }
            return x.stateData != y.stateData;
        }
        
    }

    public class WorldStateObjectPair : WorldStateObject
    {
        public StateKeys key;
        public int element1;
        public int element2;

    }


    public class WorldStateContinent : WorldStateObject
    {
        // The continent being targeted
        public List< MapSystem.Continent> continents;

        public WorldStateContinent()
        {
            continents = new List<MapSystem.Continent>();
        }


    }
    
    public class WorldStateGraphList : WorldStateObject
    {
        public List<MapSystem.BoardTile> tiles = new List<MapSystem.BoardTile>();
        
        public WorldStateGraphList()
        {
            
        }
    }
    
}