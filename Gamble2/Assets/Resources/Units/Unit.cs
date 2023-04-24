using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit 
{

    // ------------------------------------------------- Variables -------------------------------------------

    int unitCount;

    //--------------------------------------------------- Properties ---------------------------------------

    public int Count
    {
        get
        {
            return Count;
        }
    }



    //----------------------------------------------- Public Functions -----------------------------------------

    /// <summary>
    /// Creates a group of units of the given size.
    /// </summary>
    /// <param name="amount">The number of units within the object.</param>
    public Unit(int amount = 0)
    {
        unitCount = amount;
    }

    /// <summary>
    /// Duplicates units  
    /// </summary>
    /// <param name="other"></param>
    public Unit(Unit other)
    {
        unitCount = other.Count;
    }


    public void Add(int unitCount)
    {
        
    }

    public static Unit operator +(Unit unit, int amount)
    {
        Unit result = new Unit(unit);
        
        // Add the amount to the result
        result.Add(amount);

        return result;
    }

}
