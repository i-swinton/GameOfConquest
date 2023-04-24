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

        Player owner;

        int unitCount;
        Unit units;
        
        //--------------------------------------------- Properties---------------------------------------------------

        public Vector3 Position 
        {
            get
            {
                return worldPosition;
            }
         }

        public int UnitCount
        {
            get
            {
                return unitCount;
            }
        }

        public Player Owner
        {
            get
            {
                return owner;
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

        /// <summary>
        /// Adds addtional units to the tile. If no units are on the tile, fills the tiles.
        /// </summary>
        /// <param name="newUnits">The additional units being added. </param>
        public void AddUnits(Unit newUnits)
        {
            if (units == null)
            {
                units = newUnits;
            }
            else
            {
                units += newUnits;
            }
        }

        /// <summary>
        /// Add additional units to a tile.
        /// </summary>
        /// <param name="additionalUnits">The units being moved to the tilr</param>
        public void Fortify(int additionalUnits)
        {
            units += additionalUnits;
        }
        
        public void ChangeOwner(Player player)
        {
            // Ignore if the players match
            if(owner == player) { return; }

            // Change the owner to the new player
            owner = player;
            
        }
    }
}
