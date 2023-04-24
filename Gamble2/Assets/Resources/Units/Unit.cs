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


    public bool IsZero
    {
        get
        {
            return Count == 0;
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
        this.unitCount += unitCount;

        if(this.unitCount <= 0)
        {
            this.unitCount = 0;
        }
    }

    public static Unit operator +(Unit unit, int amount)
    {
        Unit result = new Unit(unit);
        
        // Add the amount to the result
        result.Add(amount);

        return result;
    }

    public static Unit operator +(Unit unit1, Unit unit2)
    {
        // Copy the original units
        Unit result = new Unit(unit1);
        // Add the secondary units
        result += unit2.unitCount;

        return result;
    }

    public static Unit operator -(Unit unit, int amount)
    {
        // Copy the original units
        Unit result = new Unit(unit);

        result.Add(-amount);

        return result;
    }

    public static Unit operator -(Unit unit1, Unit unit2)
    {
        // Copy the original units
        Unit result = new Unit(unit1);
        // Add the secondary units
        result -= unit2.unitCount;

        return result;
    }

}
