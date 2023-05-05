using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class Blackboard
    {
        // Dictionary Blackboard
        public Dictionary<string, BlackboarData> blackboard = new Dictionary<string, BlackboarData>();

        public BlackboarData this[string key] => GetValue(key);

        public void UpdateValue(string key, int value)
        {
            // If it does not have a key for this value, then add that value to the dictionary
            if (!blackboard.ContainsKey(key))
            {
                BlackboarData data = new BlackboarData(value);
                blackboard.Add(key, data);

                return;
            }
            // Otherwise update the given value
            blackboard[key].SetValue(value);
        }

        public void UpdateValue(string key, MonoBehaviour value)
        {
            // If it does not have a key for this value, then add that value to the dictionary
            if (!blackboard.ContainsKey(key))
            {
                BlackboarData data = new BlackboarData(value);
                blackboard.Add(key, data);

                return;
            }
            // Otherwise update the given value
            blackboard[key].SetValue(value);
        }


        public BlackboarData GetValue(string key)
        {
            if (blackboard.ContainsKey(key))
            {
                return blackboard[key];
            }

            // Otherwise throw exception
            throw new System.Exception("The key \"" + key + "\" does not exist on the blackboard");
        }

        public int GetInt(string key)
        {
            if (blackboard.ContainsKey(key))
            {
                return blackboard[key].GetInt();
            }
            throw new System.Exception("The key \"" + key + "\" does not exist on the blackboard");
        }

        public MonoBehaviour GetComponent(string key)
        {
            if (blackboard.ContainsKey(key))
            {
                return blackboard[key].GetComponent();
            }
            throw new System.Exception("The key \"" + key + "\" does not exist on the blackboard");
        }
        public bool Contains(string key)
        {
            return blackboard.ContainsKey(key);
        }
    }


    public struct BlackboarData
    {
        public int intVal;
        public string stringVal;
        public float floatVal;

        public MonoBehaviour componentVal;


        public Vector3 vecVal;

        public enum DataType
        {
            Int,
            String,
            Float,
            Vector,

            // Unity Specific
            Component
        }

        DataType myType;

        #region Constructors
        public BlackboarData(int value)
        {
            intVal = value;
            myType = DataType.Int;

            stringVal = "";
            floatVal = 0.0f;
            vecVal = new Vector3();
            componentVal = null;
        }

        public BlackboarData(Vector3 value)
        {
            intVal = 0;
            myType = DataType.Vector;

            stringVal = "";
            floatVal = 0.0f;
            vecVal = value;
            componentVal = null;

        }


        public BlackboarData(MonoBehaviour value)
        {
            intVal = 0;
            myType = DataType.Vector;

            stringVal = "";
            floatVal = 0.0f;
            vecVal = Vector3.zero;
            componentVal = value;

        }

        #endregion
        public void SetValue(int value)
        {
            intVal = value;
        }

        public void SetValue(Vector3 value)
        {
            vecVal = value;
        }

        public void SetValue(MonoBehaviour comp)
        {
            componentVal = comp;
        }

        public int GetInt()
        {
            if (myType == DataType.Int) { return intVal; }

            throw new System.Exception("Blackboard Data accessed was not an integer.");
        }

        public Vector3 GetVector()
        {
            if (myType == DataType.Vector) { return vecVal; }

            throw new System.Exception("Blackboard Data accessed was not a vector.");
        }

        public MonoBehaviour GetComponent()
        {
            if (myType == DataType.Component) { return componentVal; }

            throw new System.Exception("Blackboard Data accessed was not a MonoBehavior");
        }
    }
}