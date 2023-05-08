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

    AI.AIPlan plan;

    public Player PlayerRef
    {
        get
        {
            return selfRef;
        }
    }

    public AI.Blackboard Blackboard
    {
        get
        {
            return bb;
        }
    }

    public AI.WorldState WorldState
    {
        get
        {
            return worldState;
        }
    }

    public AIPlayer(Player player)
    {
        // Create the blackboard
        bb = new AI.Blackboard();

        selfRef = player;

        plan = new AI.AIPlan();

        worldState = new AI.WorldState(0);
    }

    public void Initialize(Player player, GameMaster master, AIPersonality persona)
    {
        selfRef = player;

        manager = master;
        manager.onTurnBegin += OnTurnChange;

        // If we are given an actual persona, then we can use it 
        if(persona != null)
        {
            this.persona = persona;
        }

        // Set the goal
        //plan.SetGoal(new AI.Goals.ConquerContinent(1, this));
        plan.AddGoal(new AI.Goals.ConquerContinent(1, this));
        plan.AddGoal(new AI.Goals.ReinforceContinent(1, this));
        plan.SetPlayer(this);

        // Fill the action space
        plan.AddToActionSpace(new AI.Options.ClaimContinentNode());
        plan.AddToActionSpace(new AI.Options.ReinforceContinentNode());
        plan.AddToActionSpace(new AI.Options.DraftContinent());
        plan.AddToActionSpace(new AI.Options.GoToEndState());
        plan.AddToActionSpace(new AI.Options.GoToFortifyState());

        plan.FormPlan();

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
        UpdateWorldState();
    }

    public void UpdateWorldState()
    {
        // Update the game mode
        worldState[AI.StateKeys.GameState] = AI.AIAssist.Convert(GameMaster.GetInstance().GetState());
        // Draft troops
        worldState[AI.StateKeys.DraftTroops] = PlayerRef.draftTroop > 0 ? AI.States.Nonzero : AI.States.Zero;

        

        // For testing reasons, set target
        if(bb["TargetCon"].GetContinent() != null)
        {
            var targetCon = bb["TargetCon"].GetContinent();

            worldState[AI.StateKeys.TargetContinent] =(( targetCon.LastOwningPlayer != null) ? AI.States.Full : AI.States.NotFull);
        }
    }

    public void Update(float dt)
    {
        // Update the current tree
        //currentTree.Update(dt);

        // If the plan
        plan.Update(dt, this);
    }

    // Turn assistance
    public void OnTurnChange(int player)
    {
        // If the player id doesn't match, return
        if (player != PlayerRef.playerID) { return; }

        // Sense
        UpdateWorldState();

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
