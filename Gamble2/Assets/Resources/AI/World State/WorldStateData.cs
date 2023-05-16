using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{

    public enum States
    {
        // Global States
        Invalid =-1,
        Any,
        None,
        No,
        Yes,


        // Numerical
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
        HasAttacked,

        // TroopCount
        IncreaseTroops,


        // Territory
        IncreaseTerritory,

        // Goal
        

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
        Mode,

        // Owns
        Owns,
        ExistsIn,

        // Target
        TargetPlayer,
        TargetContinent,

        // Attack Data
        CanAttack,
        AttackState,

        CardGain,

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
            states[(int)StateKeys.Mode] = States.Any;

            states[(int)StateKeys.DraftTroops] = States.Any;

            states[(int)StateKeys.CanAttack] = States.Any;

            states[(int)StateKeys.CardGain] = States.Any;

            states[(int)StateKeys.Owns] = new WorldStateObjectList();
            states[(int)StateKeys.Owns].Apply(States.Any);

            states[(int)StateKeys.ExistsIn] = new WorldStateObjectList();
            states[(int)StateKeys.ExistsIn].Apply(States.Any);


            states[(int)StateKeys.TargetContinent] = States.Any;
            states[(int)StateKeys.TargetTroopCount] = States.Any;
            states[(int)StateKeys.TroopCount] = States.Any;
            //states[(int)StateKeys.] = States.Any;
            states[(int)StateKeys.AttackState] = States.Any;
        }

        /// <summary>
        /// Copy constructor of world state
        /// </summary>
        /// <param name="other"></param>
        public WorldState(WorldState other)
        {
            // Create the array
            states = new WorldStateObject[(int)StateKeys.Count];

            // Fill each of the slots
            states[(int)StateKeys.GameState] = other[StateKeys.GameState];
            states[(int)StateKeys.Mode] = other[StateKeys.Mode];

            states[(int)StateKeys.DraftTroops] = other[StateKeys.DraftTroops];

            states[(int)StateKeys.CanAttack] = other[StateKeys.CanAttack];

            // Match the own keys
            states[(int)StateKeys.Owns] = new WorldStateObjectList((WorldStateObjectList)other.states[(int)StateKeys.Owns]);
            //states[(int)StateKeys.Owns].Apply(States.None);

            states[(int)StateKeys.ExistsIn] = new WorldStateObjectList((WorldStateObjectList)other.states[(int)StateKeys.ExistsIn]);
            states[(int)StateKeys.TargetContinent] = other[StateKeys.TargetContinent];
            states[(int)StateKeys.TargetTroopCount] = other[StateKeys.TargetTroopCount];
            states[(int)StateKeys.TroopCount] = other[StateKeys.TroopCount];
            //states[(int)StateKeys.] = States.Any;
            states[(int)StateKeys.AttackState] = other[StateKeys.AttackState];
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
                states[(int)key].Apply( value);
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

        public void SetValue(StateKeys key, States value)
        {
            states[(int)key].Apply(value);
        }

        public void AddValue(StateKeys key, object value)
        {
            ((WorldStateObjectList)states[(int)key]).Add(value);
        }

        public void RemoveValue(StateKeys key, object value)
        {
            ((WorldStateObjectList)states[(int)key]).Remove(value);
        }

        public override string ToString()
        {
            return base.ToString();
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

        public virtual bool CompareTo(WorldStateObject y)
        {
            if (this is null) { return y is null; }
            else if (y is null) { return this is null; }

            if (this.stateData == States.Invalid || y.stateData == States.Invalid) { return false; }


            // Since it is state data
            if (y.stateData == States.Any) { return true; }

            // Then just match the x/y
            return stateData == y.stateData;
        }

        public static bool operator==(WorldStateObject x, WorldStateObject y)
        {
            if (x is null) { return y is null; }
            else if (y is null) { return x is null; }
            return x.CompareTo(y);
        }

        public static bool operator!=(WorldStateObject x, WorldStateObject y)
        {
            if (x is null) { return !(y is null); }
            else if (y is null) { return !(x is null); }

            if (x.stateData == States.Invalid || y.stateData == States.Invalid) { return false; }


            // IF the second is any, than the two match
            if (y.stateData == States.Any) { return false; }
            return x.stateData != y.stateData;
        }

        public virtual void Apply(States state)
        {
            stateData = state;
        }

        public override string ToString()
        {
            return stateData.ToString();
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
    
    public class WorldStateObjectList : WorldStateObject
    {
        public List<object> list = new List<object>();

        public WorldStateObjectList()
        {
            // Start the list as none
            stateData = States.Any;
        }

        public WorldStateObjectList(WorldStateObjectList other)
        {
            // Match the stateData
            stateData = other.stateData;
            // Copy all of the elements from one list to the other
            for(int i=0; i < other.list.Count; ++i)
            {
                // Add all the elements into the list
                Add(other.list[i]);
            }
        }


        public override void Apply(States state)
        {
            // Only allow to states
            if(state != States.None && state != States.Nonzero && state != States.Any && state != States.Invalid) { return; }
            base.Apply(state);
        }

        public void Add(object obj)
        {
            list.Add(obj);
            // Auto update the state
            if(list.Count > 0 && stateData != States.Nonzero)
            {
                Apply(States.Nonzero);
            }
        }
        public void Remove(object obj)
        {
            list.Remove(obj);
            // Auto update the state
            if (list.Count <= 0)
            {
                Apply(States.None);
            }
        }

        public bool Contains(object obj)
        {
            return list.Contains(obj);
        }

        public static bool operator ==(WorldStateObjectList x, WorldStateObject y)
        {
            if (x is null) { return y is null; }
            else if (y is null) { return x is null; }

            if (x.stateData == States.Invalid || y.stateData == States.Invalid) { return false; }


            // Since it is state data
            if (y.stateData == States.Any) { return true; }

            // Then just match the x/y
            return x.stateData == y.stateData;
        }

        public static bool operator !=(WorldStateObjectList x, WorldStateObject y)
        {
            if (x is null) { return !(y is null); }
            else if (y is null) { return !(x is null); }

            if(x.stateData == States.Invalid || y.stateData == States.Invalid) { return false; }

            // IF the second is any, than the two match
            if (y.stateData == States.Any) { return false; }
            return x.stateData != y.stateData;
        }

        public override bool CompareTo(WorldStateObject y)
        {
            bool b = base.CompareTo(y);

            if (!b) { return false; }

            WorldStateObjectList z = new WorldStateObjectList((WorldStateObjectList)y);

            if(z!=null)
            {

            }
            // Check if y is a subset of x
            for (int i = 0; i < z.list.Count; ++i)
            {
                // If the element is not in x, they do not match
                if (!this.Contains(z.list[i]))
                {
                    return false;
                }
            }

            return true;


        }

        public static bool operator ==(WorldStateObjectList x, WorldStateObjectList y)
        {
            if (x is null) { return y is null; }
            else if (y is null) { return x is null; }

            if (x.stateData == States.Invalid || y.stateData == States.Invalid) { return false; }


            // Since it is state data
            if (y.stateData == States.Any) { return true; }

            

            // If the x and y state data don't match, then these two do not match
            if(x.stateData != y.stateData) { return false; }

           

            // Then just match the x/y
            return x.stateData == y.stateData;
        }

        public static bool operator !=(WorldStateObjectList x, WorldStateObjectList y)
        {
            if (x is null) { return !(y is null); }
            else if (y is null) { return !(x is null); }

            if (x.stateData == States.Invalid || y.stateData == States.Invalid) { return false; }


            if (y.stateData == States.Any) { return false; }

            // If the x and y state data don't match, then these two do not match
            if (x.stateData != y.stateData) { return true; }

            // Check if y is a subset of x
            for (int i = 0; i < y.list.Count; ++i)
            {
                // If the element is not in x, they do not match
                if (!x.Contains(y.list[i]))
                {
                    return true;
                }
            }


            return x.stateData != y.stateData;
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