using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapSystem
{


    public class Graph
    {
        // A list of all nodes within the graph
        protected List<Node> nodes;

        // 
        protected int nextNodeIndex;

        //----------------------- Public Properties ---------------------------------

        /// <summary>
        /// Get the number of nodes within the graph.
        /// </summary>
        public int Count
        {
            get
            {
                // Since indexing is zero-based, the next node index is the count
                return nextNodeIndex;
            }
        }

        //----------------------- Public Functions ---------------------------------

        public Graph()
        {
            nodes = new List<Node>();

            // Initialize the next node index
            nextNodeIndex = 0;
        }

        public virtual void Connect(int id1, int id2)
        {
            // Fail loudly
            if(Count <= id1 || Count <=id2)
            {
#if UNITY_EDITOR
                throw new NodeNotFoundException("Unable to find " + id1 + " and/or " + id2 + " for connection.");
#else
                return;
#endif
            }

            // If no problems, connect the two
            this[id1].AddConnection(this[id2]);
        }

        /// <summary>
        /// Finds a node on the graph given that node's id number
        /// </summary>
        /// <param name="id">The id number of the node being searched for.</param>
        /// <returns>If the node is found, returns the requested node. Otherwise, returns null.</returns>
        public Node Find(int id)
        {

            // Perform the find operation
            Node node = PrivateFind(id);

            if(node != null) { return node; }

            // Fail silently when built, while failing loudly in editor
#if UNITY_EDITOR
            throw new NodeNotFoundException("Unable to find node at index " + id, id);
#else
            // If unable to find the node, returns null

            return null;

#endif
        }


        /// <summary>
        /// Checks if the graph contains a node given an id number for the node.
        /// </summary>
        /// <param name="id">The id number of node being requested.</param>
        /// <returns>If the node is within the graph, return true. Otherwise, return false.</returns>
        public bool Contains(int id)
        {
            return PrivateFind(id) != null;
        }

        /// <summary>
        /// Finds a node on the graph given that node's id number
        /// </summary>
        /// <param name="id">The id number of the node being searched for.</param>
        /// <returns>If the node is found, returns the requested node. Otherwise, returns null.</returns>
        public Node this[int id] =>Find(id);

        /// <summary>
        /// Create a new node on the graph.
        /// </summary>
        /// <returns>Returns a reference to that newly created node.</returns>
        public virtual Node MakeNode()
        {
            // Create new node and increment the next index
            Node node = new Node(nextNodeIndex++);

            // Add the new node to the list
            nodes.Add(node);

            // Return the new node
            return node;
        }


        // --------------------- Private Functions ----------------------------------

        /// <summary>
        /// What truly performs the find operation using the given id. 
        /// </summary>
        /// <param name="id">The id number of the node being searched for</param>
        /// <returns>If the node is found, returns the requested node. Otherwise, returns null.</returns>
        Node PrivateFind(int id)
        {
            // Look through the entirety of the nodes on the graph
            for (int i = 0; i < nodes.Count; ++i)
            {
                // If the ids match, report the found node
                if (nodes[i].ID == id)
                {
                    return nodes[i];
                }
            }

            // If unable to find, return null
            return null;
        }

    }

    public class Node
    {
        //------------------------ Variables ---------------------------------------

        List<Node> connections;

        // The unique identifier of a node
        int myId;

        //----------------------- Public Properties ---------------------------------

        /// <summary>
        /// The unique identifier which each node on a graph has.
        /// </summary>
        public int ID
        {
            get
            {
                return myId;
            }
        }

        public List<Node> Neighbors
        {
            get
            {
                return connections;
            }
        }

        //----------------------- Public Functions ---------------------------------

        public Node(int id)
        {
            myId = id;

            // Create the list of nodes
            connections = new List<Node>();
        }

        public Node(Node other)
        {
            // Clone the ID
            myId = other.ID;

            // NOTE: This will share the reference, allowing both the clone and the original to access the same list of connections
            connections = other.connections;
        }

        public void AddConnection(Node other)
        {
            // Ensure we are not adding redundant connections
            if (IsConnectedTo(other)) { return; }

            // Otherwise, perform the operation
            connections.Add(other);
            other.connections.Add(this);
        }

        public bool IsConnectedTo(Node other)
        {
            // Look through connections for the other node
            if(connections.Contains(other))
            {
                return true;
            }

            // If we are unable to find other, return false
            return false;
        }
    }


    //---------------------------------Errors ------------------------------------------------
#region Exceptions
    [System.Serializable]
    public class NodeNotFoundException : System.Exception
    {

        public int NodeId{get;}

        public NodeNotFoundException() { }

        public NodeNotFoundException(string message)
            : base(message) { }

        public NodeNotFoundException(string message, System.Exception inner)
            : base(message, inner) { }

        public NodeNotFoundException(string message, int nodeID): base(message)
        {
            NodeId = nodeID;
        }


    }
#endregion
}
