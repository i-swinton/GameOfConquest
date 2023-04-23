using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapSystem
{


    public class Graph
    {
        List<Node> nodes;

        //----------------------- Public Properties ---------------------------------

        //----------------------- Public Functions ---------------------------------

        public Graph()
        {
            nodes = new List<Node>();
        }

        public Node Find(int id)
        {
            // Look through the entirety of the nodes on the graph
            for(int i =0; i < nodes.Count;  ++i)
            {
                // If the ids match, report the found node
                if(nodes[i].ID == id)
                {
                    return nodes[i];
                }
            }

            // Fail silently when built, while failing loudly in editor
#if UNITY_EDITOR
            throw new NodeNotFoundException("Unable to find node at index " + id, id);
#else
            // If unable to find the node, returns null

            return null;

#endif
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
