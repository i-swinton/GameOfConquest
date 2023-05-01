using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RNG 
{
    static int seed;

    static long holdRand;


    //int rand(void)
    //{
    //    return (((holdrand = holdrand * 214013L +
    //            2531011L) >> 16) & 0x7fff);
    //}

    public static void SeedRandom(int newSeed)
    {
        seed = newSeed;
        holdRand = (long)seed;
    }

    public static int rand()
    {
        holdRand = (((holdRand * 214013L +
                2531011L) >> 16) & 0x7fff);

        return (int)holdRand;
    }

    public static int Range(int inclusiveMin, int exclusiveMax)
    {
        // x % (max-min) | W
        // x + min
        return (rand() % (exclusiveMax - inclusiveMin)) + inclusiveMin;
    }

    public static float Range(float inclusiveMin, float inclusiveMax)
    {
        return (
            (float)((rand()) % (inclusiveMax - inclusiveMin))+
            ((rand())/1000.0f)) 
            + inclusiveMin;
    }

    public static int Roll(int inclusiveMin, int inclusiveMax, bool isOnlineSynced)
    {
        if (isOnlineSynced)
        {
            return Roll(inclusiveMin, inclusiveMax);
        }
        return Random.Range(inclusiveMin, inclusiveMax + 1);
    }

    public static float Roll(float inclusiveMin, float inclusiveMax, bool isOnlineSynced)
    {
        if(isOnlineSynced)
        {
            return Roll(inclusiveMin, inclusiveMax);
        }
        return Random.Range(inclusiveMin, inclusiveMax);
    }

    public static int Roll(int inclusiveMin, int inclusiveMax)
    {
        return Range(inclusiveMin, inclusiveMax + 1);
    }

    public static float Roll(float inclusiveMin, float inclusiveMax)
    {
        return Range(inclusiveMin, inclusiveMax);
    }


}
