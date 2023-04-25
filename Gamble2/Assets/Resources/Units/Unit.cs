using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit 
{
    // ------------------------------------------------- Declarations ---------------------------------------
    public enum Type
    {
        Standard
    }

    // ------------------------------------------------- Variables -------------------------------------------

    int unitCount;
    Type unitType;
    //--------------------------------------------------- Properties ---------------------------------------

    public int Count
    {
        get
        {
            return unitCount;
        }
    }

    public Type UnitType
    {
        get
        {
            return unitType;
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
    /// Creates a group of units of a given size and unit type
    /// </summary>
    /// <param name="amount">The number of units within the object. </param>
    /// <param name="type">The type of unit the unit uses.</param>
    public Unit(int amount, Type type)
    {
        unitType = type;

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
