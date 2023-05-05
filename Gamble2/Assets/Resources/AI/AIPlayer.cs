using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    Player selfRef;
    GameMaster manager;
    [SerializeField]
    AIPersonality persona;

    // The current world state
    AI.WorldState worldState;

    // The blackboard
    AI.Blackboard bb;

    AI.BehaviorTree currentTree;

    public Player PlayerRef
    {
        get
        {
            return selfRef;
        }
    }

    private void Awake()
    {
        // Create the blackboard
        bb = new AI.Blackboard();
    }

    public void Initialize(Player player, GameMaster master, AIPersonality persona)
    {
        selfRef = player;

        manager = master;

        // If we are given an actual persona, then we can use it 
        if(persona != null)
        {
            this.persona = persona;
        }
    }

}
