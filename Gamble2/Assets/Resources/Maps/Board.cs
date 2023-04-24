using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapSystem
{


    public class Board:Graph
    {

        // Used to hold game relevant information about the graph. 
        // May want to have it also hold a disjoint graph to quickly check if two board tiles are connected, but that is a later issue


        // ------------------------------------ Public Functions ---------------------------------------------------




        //---------------------------------------- Overridden Functions --------------------------------------------
        /// <summary>
        /// Adds a boardTile to the board.
        /// </summary>
        /// <returns>Returns a reference to the board.</returns>
        public override Node MakeNode()
        {

            // Create new node and increment the next index
            BoardTile node = new BoardTile(nextNodeIndex++, Vector3.zero);

            // Add the new node to the list
            nodes.Add(node);
            return node;
        }

        public Node MakeNode(Vector3 worldPosition)
        {
            // Create new node and increment the next index
            BoardTile node = new BoardTile(nextNodeIndex++, worldPosition);

            // Add the new node to the list
            nodes.Add(node);
            return node;
        }

    }


}
