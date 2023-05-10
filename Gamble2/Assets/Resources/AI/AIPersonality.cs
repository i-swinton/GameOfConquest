using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Personality",menuName ="AI/Personality")]
public class AIPersonality : ScriptableObject
{
    public List<AI.GoalTypes> goals;
    public List<AI.Options.ActionTypes> actions;

}
