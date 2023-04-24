using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusBase : ScriptableObject
{
    public enum BonusType
    {
        Unit,
    }

    // ------------------------------------------ Variables --------------------------------

    [SerializeField] BonusType bType;


    // ------------------------------------------- Properties ----------------------------------

    /// <summary>
    /// Get the type of bonus which this bonus counts as.
    /// </summary>
    public BonusType MyType
    {
        get
        {
            return bType;
        }
    }

    // ----------------------------------------- Public Functions -----------------------------------

    public virtual void Apply()
    {

    }

}