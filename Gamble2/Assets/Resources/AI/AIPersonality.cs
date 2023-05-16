using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Personality",menuName ="AI/Personality")]
public class AIPersonality : ScriptableObject
{
    [Header("Name")]
    public string personaName;

    [Header("Planning")]
    public List<AI.GoalTypes> goals;
    public List<AI.Options.ActionTypes> actions;

    [Header("Attack Preferences")]
    public int preferredAttackNumber;


    public void InitBot(AIPlayer player)
    {
        // Set the players maximum number of attacks, with a minimum of 1
        player.Blackboard.UpdateValue("MaxNumOfAttacks", System.Math.Max( preferredAttackNumber,1));
    }


    public override string ToString()
    {
        return personaName;
    }
}
