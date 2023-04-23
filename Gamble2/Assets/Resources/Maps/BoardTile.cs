using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MapSystem
{

    public class BoardTile : Node
    {
        // Board Tile can hold the data needed for each tile,
        // I.E. which player occupies it, how many troops are on it, etc.
        // Also spacial information?

        // -------------------------------------------- Variables ---------------------------------------------------
        Vector3 worldPosition;

        //--------------------------------------------- Properties---------------------------------------------------

        public Vector3 Position 
        {
            get
            {
                return worldPosition;
            }
         }

        // -------------------------------------------- Public Functions ---------------------------------------------
        public BoardTile(int id, Vector3 worldPosition): base(id)
        {

            this.worldPosition = worldPosition;
        }

        /// <summary>
        /// Moves the central position of the board tile to another point in the world.
        /// </summary>
        /// <param name="newPosition">The new position the world position is being set to</param>
        public void MoveTo(Vector3 newPosition)
        {
            worldPosition = newPosition;
        }



    }
}
