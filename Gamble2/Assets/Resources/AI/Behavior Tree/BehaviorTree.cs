using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{

    public class BehaviorTree
    {
        TreeNode root;

        public AIPlayer player;

        //----------------------------------- Public Function -------------------------------------
        
        /// <summary>
        /// Clones the behavior tree
        /// </summary>
        /// <param name="other"></param>
        public BehaviorTree(BehaviorTree other)
        {
            other.root = other.root.Clone();
        }

        
        public void SetRoot(TreeNode root)
        {
            this.root = root;
        }

        public void Update(float dt)
        {
            // Don't update the root when they died
            if(root == null) { return; }

            // Update the tree
            root.Update(dt);
        }
    }

    public abstract class TreeNode
    {
        public enum NodeState
        {
            Complete,
            Working,
            Failed
        }
        public enum NodeType
        {
            Decorator,
            Control,
            Leaf
        }

        protected List<TreeNode> children;

        // The node which this node references. If no parent, this is the root node
        TreeNode parent;

        // The tree which the node is a part of. May be used by certain nodes
        protected BehaviorTree tree;

        protected NodeType nodeType;

        // Used to check if the current iteration is the first time entering the node
        bool firstTimeEntry = true;



        public bool FirstTimeEntry
        {
            get
            {
                return firstTimeEntry;
            }
        }

        public abstract string Name
        {
            get;
        }

        public NodeType MyType
        {
            get
            {
                return nodeType;
            }
        }

        public bool IsRoot
        {
            get
            {
                return parent == null;
            }
        }

        public TreeNode Parent
        {
            get
            {
                return parent;
            }
        }

        public TreeNode(TreeNode _parent, BehaviorTree _tree)
        {
            // Create the list of children
            children = new List<TreeNode>();

            parent = _parent;

            tree = _tree;
        }

        /// <summary>
        /// Begins the node
        /// </summary>
        public virtual void Start()
        {

        }

        public abstract NodeState Update(float dt);

        public virtual void End()
        {

        }

        public abstract void AddNode(TreeNode node);


        /// <summary>
        /// Actions performed whenever leaving a tree node. Performed on either exit or cancel.
        /// </summary>
        public virtual void CleanUp()
        {
            firstTimeEntry = true;
        }

        /// <summary>
        ///  Actions performed if the node does not complete the action and is instead interrupted
        /// </summary>
        public virtual void Cancel()
        {
            // Perform the required clean up actions
            CleanUp();
        }

        /// <summary>
        /// Clones the tree and all of its children
        /// </summary>
        /// <returns></returns>
        public abstract TreeNode Clone();
    }


}