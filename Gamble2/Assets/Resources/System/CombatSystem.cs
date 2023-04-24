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
        List<int> attackRolls = new List<int>();
        List<int> defenderRolls = new List<int>();


        switch (combatType)
        {
            case Combat.CombatRollType.Single:
                {
                    // Both units roll one die
                    attackRolls = RollDice(1, attackTile.Bonuses);
                    defenderRolls = RollDice(1, attackTile.Bonuses);

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

    /// <summary>
    /// Rolls the dice for calculating the values for combat
    /// </summary>
    /// <param name="count"></param>
    /// <param name="bonuses"></param>
    /// <returns></returns>
    List<int> RollDice(int count, List<BonusBase> bonuses)
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
                        for(int i=0; i < dieBonus.NumberOfDice;  ++i)
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
