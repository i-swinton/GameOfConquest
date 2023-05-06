using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer 
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

    public AIPlayer(Player player)
    {
        // Create the blackboard
        bb = new AI.Blackboard();

        selfRef = player;
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

        // Create the behavior tree
        currentTree = new AI.BehaviorTree(this);

        // Build the standard behavior tree

        ///                        Sequence
        ///                        /             \
        ///               Sequence             Sequence
        ///             /       \           /      |    \
        ///        Sequence
        ///        /       \
        ///       Claim     End  Reinforce    Draft  Attack Foritfy

        AI.SequenceNode root = new AI.SequenceNode(null, currentTree);


        // Left Branch
        AI.SequenceNode seqL1 = new AI.SequenceNode(root, currentTree);
       // AI.SequenceNode seqL2 = new AI.SequenceNode(seqL1, currentTree);

        root.AddNode(seqL1);//root.AddNode(seqL2);

        seqL1.AddNode(new AI.ClaimRandomNode(seqL1, currentTree));

        seqL1.AddNode(new AI.ReinforceRandomNode(seqL1, currentTree));

        // Right Branch
        AI.SequenceNode seqR1 = new AI.SequenceNode(root, currentTree);

        root.AddNode(seqR1);

        seqR1.AddNode(new AI.DraftRandomNode(seqR1, currentTree));

        // Set up the selector
        AI.SelectorNode selR1 = new AI.SelectorNode(seqR1, currentTree);
        seqR1.AddNode(selR1);

        selR1.AddNode( new AI.AttackRandomNode(selR1, currentTree));
        selR1.AddNode(new AI.EndTurnNode(selR1, currentTree));

        //  Set up the fortify
        seqR1.AddNode(new AI.EndTurnNode(seqR1, currentTree));

        // Initialize the root
        currentTree.SetRoot(root);
    }

    public void Update(float dt)
    {
        // Update the current tree
        currentTree.Update(dt);
    }

}



namespace AI
{
    public class AIPlayerData
    {
        int playerCount;

        public AIPlayerData(int playerCount)
        {
            this.playerCount = playerCount;
        }


        public int PlayerCount
        {
            get
            {
                return playerCount;
            }
        }
       
    }
}
