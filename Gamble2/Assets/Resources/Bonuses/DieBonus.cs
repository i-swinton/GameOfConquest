using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DiceBonus", menuName = "Bonus/Dice")]

public class DieBonus : BonusBase
{
    [SerializeField] int numOfDice;
    
    public int NumberOfDice(MapSystem.BoardTile tile, Combat.CombatantType combatType)
    {
        return numOfDice;
    }
}
