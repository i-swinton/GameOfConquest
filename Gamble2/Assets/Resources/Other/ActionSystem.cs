using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actions {
    public class ActionList
    {
        // List of actions
        List<Action> actions;

        float timeScalar = 1.0f;


        public float TimeScalar
        {
            get
            {
                return timeScalar;
            }
            set
            {
                // Ignore input if negative
                if(value < 0) { return; }

                timeScalar = value;
            }
        }


        // Creates an action list
        public ActionList()
        {
            actions = new List<Action>();
        }

       

        public void Add(Action action)
        {
            actions.Add(action);
        }

        public void Clear()
        {
            // Cancel all the actions 
            for(int i=0; i < actions.Count; ++i)
            {
                actions[i].Cancel();
            }
            // Clear all of the actions from the list
            actions.Clear();
        }

        bool CompareGroups(int groups, ActionType at)
        {
            // Use the bit mask to test if it is included
            if((groups & 1<<((int)at)) != 0)
            {
                return true;
            }
            
            // Otherwise, it is not included
            return false;
        }


        public void Update(float deltaTime)
        {
            deltaTime *= TimeScalar;

            // The flags to be blocked
            int blockFlags = 0;

            // Iterate through all of the actions
            for(int i= 0; i < actions.Count; ++i)
            {
                // If this action is blocked, skip it
                if(CompareGroups(blockFlags, actions[i].Group))
                {
                    continue;
                }

                // Remove the action once it is complete
                if (actions[i].Update(deltaTime))
                {
                    // Remove the action from the list
                    actions.RemoveAt(i);

                    // Decrement the index operator
                    --i;

                    // Don't worry about ticking blocking on remove frame
                    continue;
                }

                // If this is blocking, block future group users
                if(actions[i].IsBlocking)
                {
                    if(actions[i].Group == ActionType.AllGroup)
                    {
                        // Max at the block flags
                        blockFlags = int.MaxValue;
                    }
                    else
                    {
                        // OR the value into the block flags
                        blockFlags |= 1 << ((int)actions[i].Group);
                    }
                }

            }
        }

    }



}

