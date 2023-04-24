using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class CombatSystem { 
    /// What is combat
    ///  Attack rolls
    ///  Defense rolls
    ///  Resolving rolls
    ///  Needs to factor in bonuses
    ///  Report losses? (Should it perform the losses)
    ///  

    // -------------------------------------------------------- Variables ---------------------------------------------------

    // ---------------------------------------------------- Properties ------------------------------------------------------



    // ------------------------------------------------------ Public Functions -------------------------------------------------





    /// <summary>
    /// Pits two tiles against one another
    /// </summary>
    /// <param name="attackTile">The tile which is declaring the attack. </param>
    /// <param name="defenderTile">The tile which is being attacekd.</param>
    /// <param name="combatType">The type of combat which is being performed.</param>

    public static void BattleTiles(MapSystem.BoardTile attackTile, MapSystem.BoardTile defenderTile, Combat.CombatRollType combatType,
        out int totalAtkLoss, out int totalDefLoss)
    {

        totalAtkLoss = 0;
        totalDefLoss = 0;

        switch (combatType)
        {
            case Combat.CombatRollType.Single:
            case Combat.CombatRollType.Double:
            case Combat.CombatRollType.Triple:
                {

                    // Perform the battle
                    PerformBattle(attackTile, defenderTile, combatType,
                        out totalAtkLoss, out totalDefLoss);

                    // Decrement the troops
                    attackTile.KillUnits(totalAtkLoss);
                    defenderTile.KillUnits(totalDefLoss);

                    break;
                } // Repeat combat until someone loses
            case Combat.CombatRollType.Blitz:
                {
                    while(attackTile.UnitCount > 1 && defenderTile.UnitCount > 0)
                    {
                        // Perform the battle
                        int aLoss; int defLoss;
                        PerformBattle(attackTile, defenderTile, combatType,
                            out aLoss, out defLoss);

                        // Decrement the troops
                        attackTile.KillUnits(aLoss);
                        defenderTile.KillUnits(defLoss);

                        // Update the total loss
                        totalAtkLoss += aLoss;
                        totalDefLoss += defLoss;
                    }
                    break;
                }
        }

    }

    static void PerformBattle(MapSystem.BoardTile attackTile, MapSystem.BoardTile defenderTile, Combat.CombatRollType combatType, out int attackerLosses, out int defenderLosses)
    {
        List<int> attackRolls;
        List<int> defenderRolls;

        
        // Perform the rolls based on the combat type
        switch (combatType)
        {
            case Combat.CombatRollType.Single:
                {
                    // Both units roll one die
                    attackRolls = RollDice(1, attackTile.Bonuses, attackTile, Combat.CombatantType.Attacker);
                    defenderRolls = RollDice(2, defenderTile.Bonuses, defenderTile, Combat.CombatantType.Defender);
                    break;
                }
            case Combat.CombatRollType.Double:
                {
                    // Both units roll two dice
                    attackRolls = RollDice(2, attackTile.Bonuses, attackTile, Combat.CombatantType.Attacker);
                    defenderRolls = RollDice(2, defenderTile.Bonuses, defenderTile, Combat.CombatantType.Defender);

                    break;
                }
            case Combat.CombatRollType.Triple:
            default:
                {
                    // Attacker rolls three die, Defender rolls two
                    attackRolls = RollDice(3, attackTile.Bonuses, attackTile, Combat.CombatantType.Attacker);
                    defenderRolls = RollDice(2, defenderTile.Bonuses, defenderTile, Combat.CombatantType.Defender);
                    break;
                }

              

        }
        // Sort the dice
        attackRolls.Sort((x, y) => RollCompare(x, y));
        defenderRolls.Sort((x, y) => RollCompare(x, y));

        // Resolve combat 
        EvaluateCombat(attackTile, defenderTile, attackRolls, defenderRolls,
            combatType, out attackerLosses, out defenderLosses);


    }

    static void EvaluateCombat(MapSystem.BoardTile attacker, MapSystem.BoardTile defender, List<int> attackRolls, 
        List<int> defenseRolls, Combat.CombatRollType rollType, out int attackerLosses, out int defenderLosses)
    {
        // Clamp the roll types     
        int numberOfUnits = Mathf.Clamp( (int)rollType,1,2);

        // You can only deal as much damage as the number of defenders
        if(defender.UnitCount < numberOfUnits)
        {
            numberOfUnits = defender.UnitCount ;
        }
        

        // Initialize the losses
        attackerLosses = 0;
        defenderLosses = 0;

        // Evaluate the losses of war
        for (int i =0; i < numberOfUnits; ++i)
        {

            if(defenseRolls[i] >= attackRolls[i])
            {
                ++attackerLosses;
            }
            else
            {
                ++defenderLosses;
            }
        }


    }


    /// <summary>
    /// Compares two rolls to one another.
    /// </summary>
    /// <param name="roll1">The first roll.</param>
    /// <param name="roll2">The second roll</param>
    /// <returns>If roll1 is greater than roll2, returns -1. Otherwise, returns 1.</returns>
    static int RollCompare(int roll1, int roll2)
    {
        return roll1 > roll2 ? -1 : 1;
    }

    /// <summary>
    /// Rolls the dice for calculating the values for combat
    /// </summary>
    /// <param name="count">The number of rolls to perform</param>
    /// <param name="bonuses">The bonuses which the tile has</param>
    /// <param name="combatantType">Is this the attacker or the defender?</param>
    /// <param name="tile">The tile which is responsible for the rolling.</param>
    /// <returns></returns>
    static List<int> RollDice(int count, List<BonusBase> bonuses, MapSystem.BoardTile tile,Combat.CombatantType combatantType )
    {
        // Clamp the count to the unit count
        if(count > tile.UnitCount)
        {
            count = tile.UnitCount;
        }


        // A list of the rolls
        List<int> rolls = new List<int>();

        // Roll the normal dice
        for(int i =0; i < count; ++i)
        {
            rolls.Add(RNG.Roll(1, 6));
        }

        // Apply the bonus effects
        foreach(BonusBase bonus in bonuses)
        {
            switch(bonus.MyType)
            {
                // Add extra dice to attacking/defending with this bonus
                case BonusBase.BonusType.ExtraDie:
                    {
                        DieBonus dieBonus = (DieBonus)bonus;
                        for(int i=0; i < dieBonus.NumberOfDice(tile,combatantType);  ++i)
                        {
                            rolls.Add(RNG.Roll(1, 6));
                        }
                        break;
                    }
            }
        }

        return rolls;
    }
}
