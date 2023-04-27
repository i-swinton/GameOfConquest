using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LossRates
{
    static Dictionary<(int,int), (float,float)> lossRates = new Dictionary<(int,int), (float,float)>();

    public static (float attackLoss ,float defenseLoss) GetLossRate(int AttackDice, int DefenseDice)
    {
        if(lossRates == null)
        {
            GenDict();
        }

        return lossRates[(AttackDice, DefenseDice)];
        
    }


    //Preditces losses based on number of defenders
    public static float GetLosses(int Defenders, int AttackDice, int DefenseDice)
    {
        //Gets conversion rates
        (float attackLoss, float defenseLoss) rates = GetLossRate(AttackDice, DefenseDice);

        return (Defenders / rates.defenseLoss) * rates.attackLoss; 
    }

    static void GenDict()
    {
        lossRates.Add((1, 1), (0.583855f, 0.416145f));
        lossRates.Add((1, 2), (0.746005f, 0.253995f));
        lossRates.Add((1, 3), (0.82682f, 0.17318f));
        lossRates.Add((1, 4), (0.874119f, 0.125881f));
        lossRates.Add((1, 5), (0.905149f, 0.094851f));
        lossRates.Add((1, 6), (0.926838f, 0.073162f));
        lossRates.Add((2, 1), (0.420944f, 0.579056f));
        lossRates.Add((2, 2), (1.219244f, 0.780756f));
        lossRates.Add((2, 3), (1.493192f, 0.506808f));
        lossRates.Add((2, 4), (1.64594f, 0.35406f));
        lossRates.Add((2, 5), (1.740517f, 0.259483f));
        lossRates.Add((2, 6), (1.803324f, 0.196676f));
        lossRates.Add((3, 1), (0.340024f, 0.659976f));
        lossRates.Add((3, 2), (0.921301f, 1.078699f));
        lossRates.Add((3, 3), (1.892016f, 1.107984f));
        lossRates.Add((3, 4), (2.249219f, 0.750781f));
        lossRates.Add((3, 5), (2.464575f, 0.535425f));
        lossRates.Add((3, 6), (2.604933f, 0.395067f));
        lossRates.Add((4, 1), (0.292573f, 0.707427f));
        lossRates.Add((4, 2), (0.750106f, 1.249894f));
        lossRates.Add((4, 3), (1.472701f, 1.527299f));
        lossRates.Add((4, 4), (2.591816f, 1.408184f));
        lossRates.Add((4, 5), (3.014598f, 0.985402f));
        lossRates.Add((4, 6), (3.28662f, 0.71338f));
        lossRates.Add((5, 1), (0.260971f, 0.739029f));
        lossRates.Add((5, 2), (0.643997f, 1.356003f));
        lossRates.Add((5, 3), (1.213178f, 1.786822f));
        lossRates.Add((5, 4), (2.070684f, 1.929316f));
        lossRates.Add((5, 5), (3.308247f, 1.691753f));
        lossRates.Add((5, 6), (3.787494f, 1.212506f));
        lossRates.Add((6, 1), (0.239671f, 0.760329f));
        lossRates.Add((6, 2), (0.572597f, 1.427403f));
        lossRates.Add((6, 3), (1.042727f, 1.957273f));
        lossRates.Add((6, 4), (1.718998f, 2.281002f));
        lossRates.Add((6, 5), (2.702867f, 2.297133f));
        lossRates.Add((6, 6), (4.046346f, 1.953654f));
    }

}
