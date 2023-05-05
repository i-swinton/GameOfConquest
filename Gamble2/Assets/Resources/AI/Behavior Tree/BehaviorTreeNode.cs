using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    // Control Flow 

    public abstract class ControlNode: TreeNode
    {
        protected int index; 

        
        public ControlNode(TreeNode _parent, BehaviorTree _tree) :base(_parent,_tree)
        {
            index = 0;
            nodeType = NodeType.Control;
        }

        public override void AddNode(TreeNode node)
        {
            children.Add(node);
        }


    }

    public class SequenceNode : ControlNode
    {
        /// <summary>
        /// Creates a sequence node
        /// </summary>
        /// <param name="_parent">The parent of the node</param>
        /// <param name="_tree">The tree the node is a part of.</param>
        public SequenceNode(TreeNode _parent, BehaviorTree _tree) :base(_parent, _tree)
        {
        }

        public override string Name => "Sequence Node";

        /// <summary>
        /// Clones the node and its children
        /// </summary>
        /// <returns>Returns the cloned tree node</returns>
        public override TreeNode Clone()
        {
            SequenceNode clone = new(Parent, tree);
            // Loop through the children for cloning

            for(int i=0; i < children.Count; ++i)
            {
                // Recurse down the tree to add the children
                clone.AddNode(children[i].Clone());
            }

            // Return the clone
            return clone;
        }

        public override NodeState Update(float dt)
        {
            // If we have no children, then we don't need to worry about updating anything and return true
            if (children.Count == 0) { return NodeState.Complete; }

            // If our current index is beyond the bounds of the array, we assume we have completed the sequenece and should return true
            if (index >= children.Count)
            {
                // Reset the index for future use
                index = 0;
                // Return sequence complete
                return NodeState.Complete;
            }

            // If the first time entering the node, set it all up
            if (children[index].FirstTimeEntry)
            {
                children[index].Start();
            }

            // Otherwise, update the current index child
            NodeState state = children[index].Update(dt);

            // If the update fails, return failed and reset the index
            if (state == NodeState.Failed)
            {
                // Reset the index for future use
                index = 0;

                return NodeState.Failed;
            } // If the node completed, queue up the next child for updating
            else if (state == NodeState.Complete)
            {
                // Exit the current state
                children[index].End();
                ++index;


            }

            // Return the tree as running
            return NodeState.Working;
        }
    }
    
    public class SelectorNode : ControlNode
    {
        public SelectorNode(TreeNode _parent, BehaviorTree _tree): base(_parent, _tree)
        {
            
        }

        public override string Name => "Selector";

        public override NodeState Update(float dt)
        {
            // If we have no children, then we don't need to worry about updating anything and return true
            if (children.Count == 0) { return NodeState.Complete; }

            // If our current index is beyond the bounds of the array, we failed to complete any of our children
            if (index >= children.Count)
            {
                // Reset the index for future use
                index = 0;
                // Return selector failed
                return NodeState.Failed;
            }
            // If the first time entering the node, set it all up
            if (children[index].FirstTimeEntry)
            {
                children[index].Start();
            }

            // Otherwise, update the current index child
            NodeState state = children[index].Update(dt);

            // If the update completes, report to parent
            if (state == NodeState.Complete)
            {
                // Reset the index for future use
                index = 0;

                return NodeState.Complete;
            } // If the node failed, queue up the next child for updating
            else if (state == NodeState.Failed)
            {
                // Exit the current state
                children[index].End();
                ++index;
            }

            // Return the tree as running
            return NodeState.Working;
        }


        /// <summary>
        /// Clones the node and its children
        /// </summary>
        /// <returns>Returns the cloned tree node</returns>
        public override TreeNode Clone()
        {
            SelectorNode clone = new(Parent, tree);
            // Loop through the children for cloning

            for (int i = 0; i < children.Count; ++i)
            {
                // Recurse down the tree to add the children
                clone.AddNode(children[i].Clone());
            }

            // Return the clone
            return clone;
        }
    }

    //------------------------------------------- Decorators -----------------------------------------

    public abstract class DecoratorNode:TreeNode
    {
        public DecoratorNode(TreeNode _parent, BehaviorTree _tree) : base(_parent, _tree)
        {

        }


        public override void AddNode(TreeNode node)
        {
            if(children.Count > 0) { throw new System.Exception("Decorators can only have one child"); }
        }
    }

    public class InverterNode : DecoratorNode
    {
        public InverterNode(TreeNode _parent, BehaviorTree _tree) : base(_parent, _tree)
        {
            nodeType = NodeType.Decorator;
        }

        public override string Name
        {
            get
            {
                return "Inverter Node";
            }
        }

        public override NodeState Update(float dt)
        {
            // If we have no children, report complete
            if (children.Count == 0) { return NodeState.Complete; }

            // Otherwisse, update the children
            NodeState state = children[0].Update(dt);

            // Invert the state if complete or failed. Running is not affected
            if (state == NodeState.Complete) { return NodeState.Failed; }
            if (state == NodeState.Failed) { return NodeState.Complete; }

            // Returnt eh state if it is still running
            return state;

        }

        /// <summary>
        /// Clones the node and its children
        /// </summary>
        /// <returns>Returns the cloned tree node</returns>
        public override TreeNode Clone()
        {
            InverterNode clone = new(Parent, tree);
            // Loop through the children for cloning

            for (int i = 0; i < children.Count; ++i)
            {
                // Recurse down the tree to add the children
                clone.AddNode(children[i].Clone());
            }

            // Return the clone
            return clone;
        }

    }

    //----------------------------------------------- Leaf Node----------------------------------------------------
    public abstract class LeafNode:TreeNode
    {
        public LeafNode(TreeNode _parent, BehaviorTree _tree): base(_parent, _tree)
        {

        }


        public override void AddNode(TreeNode node)
        {
            throw new System.Exception("Leaf nodes cannot have any children");
        }
    }

    public class WaitNode : LeafNode
    {

        float delay = 0.0f;
        float timeElapsed = 0.0f;
        public WaitNode(TreeNode _parent, BehaviorTree _tree, float duration) : base(_parent, _tree)
        {

            delay = duration;
        }

        public override string Name
        {
            get
            {
                return "Wait";
            }
        }

        public override void Start()
        {
            // Do the initial set up
            base.Start();

            // Set up the wait node here
            timeElapsed = 0;

        }


        public override NodeState Update(float dt)
        {
            // Tick the timer until the delay is passed
            timeElapsed += dt;
            if (timeElapsed >= delay)
            {
                return NodeState.Complete;
            }

            return NodeState.Working;
        }


        /// <summary>
        /// Clones the node and its children
        /// </summary>
        /// <returns>Returns the cloned tree node</returns>
        public override TreeNode Clone()
        {
            WaitNode clone = new(Parent, tree, delay);
            // Loop through the children for cloning

            for (int i = 0; i < children.Count; ++i)
            {
                // Recurse down the tree to add the children
                clone.AddNode(children[i].Clone());
            }

            // Return the clone
            return clone;
        }

    }

    public class DraftRandomNode : LeafNode
    {
        AIPlayer player;
        MapSystem.Board board;

        public DraftRandomNode(TreeNode _parent, BehaviorTree _tree) : base (_parent, _tree)
        {

            // Set the player
            player = _tree.player;
            board = BoardManager.instance.GetBoard();

        }

        public override string Name => "Draft Random";

        public override NodeState Update(float dt)
        {
            if(player.PlayerRef.draftTroop >0)
            {
                // Pick a random location
                int randomTile = RNG.Roll(0, board.Count - 1);
                MapSystem.BoardTile tile = board[randomTile];
                

                // Make sure that location is owned
                while(tile.Owner != player.PlayerRef)
                {
                    randomTile++;
                    if (randomTile >= board.Count)
                    {
                        randomTile = 0;
                    }

                    tile = board[randomTile];
                }
                // Add to that location
                
                GameMaster.GetInstance().DraftTile(tile);
            }
            else // If we are out of draft troops say we are complete
            {
                return NodeState.Complete;
            }

            // Otherwise we are working on it
            return NodeState.Working;
        }


        public override TreeNode Clone()
        {
            DraftRandomNode clone = new(Parent, tree);
            // Loop through the children for cloning

            for (int i = 0; i < children.Count; ++i)
            {
                // Recurse down the tree to add the children
                clone.AddNode(children[i].Clone());
            }

            // Return the clone
            return clone;
        }
    }

    public class EndTurnNode : LeafNode
    {
        public EndTurnNode(TreeNode _parent, BehaviorTree _tree) :base(_parent,_tree)
        {

        }

        public override string Name => "End Turn";

        public override NodeState Update(float dt)
        {
            // Call the force end turn function
            GameMaster.GetInstance().ForceTurnEnd();

            return NodeState.Complete;
        }

        public override TreeNode Clone()
        {
            EndTurnNode clone = new(Parent, tree);
            // Loop through the children for cloning

            for (int i = 0; i < children.Count; ++i)
            {
                // Recurse down the tree to add the children
                clone.AddNode(children[i].Clone());
            }

            // Return the clone
            return clone;
        }

    }

    public class AttackRandomNode : LeafNode
    {
        Player player;
        AIPlayer brain;

        GameMaster gm;

        MapSystem.Board board;

        MapSystem.BoardTile targetTile;

        MapSystem.BoardTile enemyTile;
        public AttackRandomNode(TreeNode _parent, BehaviorTree _tree) : base(_parent, _tree)
        {
            brain = _tree.player;
            player = brain.PlayerRef;

            gm = GameMaster.GetInstance();


        }

        public override string Name => "Attack Random";

        public MapSystem.BoardTile FindAttackableTile()
        {
            // Find random start index
            int start = RNG.Roll(0, board.Count, false) ;

            for(int i =0; i < board.Count; ++i)
            {
                // Clamp within bounds
                start++;
                start %= board.Count;

                // 
                foreach (MapSystem.BoardTile node in board[start].Neighbors)
                {
                    // See if the owner is different
                    if (node.Owner != null && node.Owner != player)
                    {
                        // Return the node
                        return node;
                    }
                }
                
            }

            // If we make it here, find the attackable tile here
            return null;

        }

        public override NodeState Update(float dt)
        {
            if(board == null)
            {
                // Get the board
                board = BoardManager.instance.GetBoard();



                // Keep working on it next loop
                return NodeState.Working;
            }

            // Next, find the next best target
            targetTile = FindAttackableTile();

            if(targetTile == null) { return NodeState.Failed; }

            // Set the attack info 

            // Challenger
            gm.AttackTile(GenerateMap.GetTile(targetTile));
            // Defender
            gm.AttackTile(GenerateMap.GetTile(enemyTile));

            // Return true
            return NodeState.Complete;
        }

        public override TreeNode Clone()
        {
            AttackRandomNode clone = new(Parent, tree);
            // Loop through the children for cloning

            for (int i = 0; i < children.Count; ++i)
            {
                // Recurse down the tree to add the children
                clone.AddNode(children[i].Clone());
            }

            // Return the clone
            return clone;
        }
    }

}