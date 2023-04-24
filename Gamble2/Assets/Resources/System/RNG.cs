using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RNG 
{
    public static int Roll(int inclusiveMin, int inclusiveMax)
    {
        return Random.Range(inclusiveMin, inclusiveMax + 1);
    }

    public static float Roll(float inclusiveMin, float inclusiveMax)
    {
        return Random.Range(inclusiveMin, inclusiveMax);
    }
}
