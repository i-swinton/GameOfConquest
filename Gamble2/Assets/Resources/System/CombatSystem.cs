using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CombatSystem : MonoBehaviour
{
    /// What is combat
    ///  Attack rolls
    ///  Defense rolls
    ///  Resolving rolls
    ///  Needs to factor in bonuses
    ///  Report losses? (Should it perform the losses)

    /// <summary>
    /// Pits two tiles against one another
    /// </summary>
    /// <param name="attackTile">The tile which is declaring the attack. </param>
    /// <param name="defenderTile">The tile which is being attacekd.</param>
    /// <param name="combatType">The type of combat which is being performed.</param>

    public void BattleTiles(MapSystem.BoardTile attackTile, MapSystem.BoardTile defenderTile, Combat.CombatRollType combatType)
    {



        switch (combatType)
        {
            case Combat.CombatRollType.Single:
                {



                    break;
                }
            case Combat.CombatRollType.Double:
                {
                    // Both units roll two die
                    break;
                }
            case Combat.CombatRollType.Triple:
                {
                    // Attacker rolls three die, Defender rolls two

                    break;
                }
            case Combat.CombatRollType.Blitz:
                {
                    break;
                }
        }

    }

    void PerformBattle(MapSystem.BoardTile attackTile, MapSystem.BoardTile defenderTile, Combat.CombatRollType combatType, out int attackerLosses, out int defenderLosses)
    {
        List<int> attackRolls;
        List<int> defenderRolls;

        

        switch (combatType)
        {
            case Combat.CombatRollType.Single:
                {
                    // Both units roll one die
                    attackRolls = RollDice(1, attackTile.Bonuses, attackTile, Combat.CombatantType.Attacker);
                    defenderRolls = RollDice(1, defenderTile.Bonuses, defenderTile, Combat.CombatantType.Defender);

                    // Sort the dice
                    attackRolls.Sort((x, y) => RollCompare(x, y));
                    defenderRolls.Sort((x, y) => RollCompare(x, y));

                    // Compare each
                    if(defenderRolls[0] >= attackRolls[0])
                    {
                        attackerLosses = 1;
                    }
                    else
                    {
                        defenderLosses = 1;
                    }

                    break;
                }
            case Combat.CombatRollType.Double:
                {
                    // Both units roll one die
                    attackRolls = RollDice(1, attackTile.Bonuses, attackTile, Combat.CombatantType.Attacker);
                    defenderRolls = RollDice(1, defenderTile.Bonuses, defenderTile, Combat.CombatantType.Defender);

                    // Sort the dice
                    attackRolls.Sort((x, y) => RollCompare(x, y));
                    defenderRolls.Sort((x, y) => RollCompare(x, y));



                    // Compare each
                    if (defenderRolls[0] >= attackRolls[0])
                    {
                        attackerLosses = 1;
                    }
                    else
                    {
                        defenderLosses = 1;
                    }

                    break;
                }
            case Combat.CombatRollType.Triple:
            default:
                {
                    break;
                }

        }

        attackerLosses = 0;
        defenderLosses = 0;


    }

    void EvaluateCombat(MapSystem.BoardTile attacker, MapSystem.BoardTile defender, List<int> attackRolls, 
        List<int> defenseRolls, Combat.CombatRollType rollType, out int attackerLosses, out int defenderLosses)
    {
        // Clamp the roll types     
        int numberOfUnits = Mathf.Clamp( (int)rollType,1,2);
        
        for(int i =0; i < numberOfUnits; ++i)
        {
            
        }

        attackerLosses = 0;
        defenderLosses = 0;
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
    List<int> RollDice(int count, List<BonusBase> bonuses, MapSystem.BoardTile tile,Combat.CombatantType combatantType )
    {
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
