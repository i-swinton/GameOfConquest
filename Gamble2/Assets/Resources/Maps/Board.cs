using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapSystem
{


    public class Board:Graph
    {


        /// <summary>
        /// Adds a boardTile to the board.
        /// </summary>
        /// <returns>Returns a reference to the board.</returns>
        public override Node MakeNode()
        {

            // Create new node and increment the next index
            BoardTile node = new BoardTile(nextNodeIndex++);

            // Add the new node to the list
            nodes.Add(node);
            return node;
        }

    }


}
