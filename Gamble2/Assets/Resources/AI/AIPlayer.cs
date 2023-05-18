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

        persona = (AIPersonality)Resources.Load("AI/Personalities/Greedy");
    }

    public void BeginPlanningSystem()
    {
        // Make
        for(int i=0; i < persona.goals.Count; ++i)
        {
            // Get the goals
            plan.AddGoal(AI.AIAssist.MakeGoal(persona.goals[i], this));
        }
        for(int i=0; i < persona.actions.Count; ++i)
        {
            plan.AddToActionSpace(AI.AIAssist.MakeAction(persona.actions[i], this));
        }
    }

    public void Initialize(Player player, GameMaster master, AIPersonality persona)
    {
        selfRef = player;

        manager = master;
        manager.onTurnBegin += OnTurnChange;

        // If we are given an actual persona, then we can use it 
        if(persona != null)
        {
            Debug.Log($"{player} has persona {persona}");
            this.persona = persona;
            persona.InitBot(this);
        }



        // Set the goal
        plan.SetPlayer(this);
        BeginPlanningSystem();


        plan.FormPlan();

#if false
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
#endif
        UpdateWorldState();
    }

    public void UpdateWorldState()
    {
        // Update the game mode
        worldState[AI.StateKeys.Mode] = AI.AIAssist.Convert(GameMaster.GetInstance().GetMode());
        worldState[AI.StateKeys.GameState] = AI.AIAssist.Convert(GameMaster.GetInstance().GetState());
        // Draft troops
        worldState[AI.StateKeys.DraftTroops] = PlayerRef.draftTroop > 0 ? AI.States.Nonzero : AI.States.Zero;
        worldState[AI.StateKeys.TroopCount] = PlayerRef.troopCount > 0 ? AI.States.Nonzero : AI.States.Zero;
        //worldState.SetValue(AI.StateKeys.TroopCount, PlayerRef.troopCount);
        //
        //worldState[AI.StateKeys.AttackState] = PlayerRef. ;

        worldState[AI.StateKeys.CardGain] = PlayerRef.canGetCard ? AI.States.Yes : AI.States.No;

        if (bb.Contains("TargetCon"))
        {
            // For testing reasons, set target
            if (bb["TargetCon"].GetContinent() != null)
            {
                var targetCon = bb["TargetCon"].GetContinent();

                worldState[AI.StateKeys.TargetContinent] = ((targetCon.IsFull) ? AI.States.Full : AI.States.NotFull);
            }
        }
        else
        {
            worldState[AI.StateKeys.TargetContinent] = AI.States.None;
        }
    }

    public void UpdateWorldState(AI.StateKeys key, AI.States state)
    {
        worldState.SetValue(key, state);
    }

    public void UpdateWorldState(AI.StateKeys key, int value)
    {
        worldState.SetValue(key, value);
    }

    public void UpdateWorldState(AI.StateKeys key, object objToAdd, bool addObject)
    {
        if (addObject)
        {
            worldState.AddValue(key, objToAdd);
        }
        else
        {
            worldState.RemoveValue(key, objToAdd);
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

        // Change last attacked to null
        Blackboard.UpdateValue("LastAttackTileID", -1);

        // Update has attacked to not have attacked
        UpdateWorldState(AI.StateKeys.AttackState, AI.States.None); 

        // Sense
        UpdateWorldState();

    }

    public void CardInCheck()
    {
        // Always card in 
        if (CardSystem.CanCardIn(PlayerRef.cards))
        {
            List<TerritoryCard> cards = CardSystem.GetCardIn(ref PlayerRef.cards);
            // Insert player for card in here
            int troopCount = CardSystem.CardIn(cards, GameMaster.GetInstance().GetPlayer(), BoardManager.instance.GetBoard());

            PlayerRef.draftTroop += troopCount;


            NotifySystem.Message($"{PlayerRef} has recieved {troopCount} units.");
        }



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
