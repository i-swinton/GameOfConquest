using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="UnitBonus",menuName ="Bonus/Unit")]
public class UnitBonus : BonusBase
{
    [Tooltip("The number of additional units added with this bonus")]
    [SerializeField] int count;



}
