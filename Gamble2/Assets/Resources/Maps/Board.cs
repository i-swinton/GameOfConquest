using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapSystem
{

    [System.Serializable]
    public class Board:Graph
    {

        // Used to hold game relevant information about the graph. 
        // May want to have it also hold a disjoint graph to quickly check if two board tiles are connected, but that is a later issue

        // --------------------------------------- Variables ------------------------------------------------------
        [Tooltip("The list of continents on the board")]
        [SerializeField] List<Continent> continents;

        // ------------------------------------ Public Functions ---------------------------------------------------

        public Board() : base()
        {
            // Create a new list of continents
            continents = new List<Continent>();
        }

        /// <summary>
        /// Get the board tile at the given index
        /// </summary>
        /// <param name="index">The index of the board tile being searched for.</param>
        /// <returns>If the tile exists within this board, returns that tile. Otherwise, returns null.</returns>
        public new BoardTile this[int index]
        {
            get
            {
                return (BoardTile)base[index];
            }
        }

        /// <summary>
        /// Add a continent to the board
        /// </summary>
        /// <param name="c">The continent being added.</param>
        public void AddContinent(Continent c)
        {
            continents.Add(c);
        }


        //---------------------------------------- Overridden Functions --------------------------------------------
        /// <summary>
        /// Adds a boardTile to the board.
        /// </summary>
        /// <returns>Returns a reference to the board.</returns>
        public override Node MakeNode()
        {

            // Create new node and increment the next index
            BoardTile node = new BoardTile(nextNodeIndex++, Vector3.zero,this);

            // Add the new node to the list
            nodes.Add(node);
            return node;
        }

        public Node MakeNode(Vector3 worldPosition)
        {
            // Create new node and increment the next index
            BoardTile node = new BoardTile(nextNodeIndex++, worldPosition, this);

            // Add the new node to the list
            nodes.Add(node);
            return node;
        }

       

        public List<BoardTile> GetConnectedTiles(int tile)
        {
            // Tiles
            List<BoardTile> closedList = new List<BoardTile>();

            // Add the initial tile to the open list
            List<BoardTile> openList = new List<BoardTile>();
            openList.Add(this[tile]);

            // Perform a Dijkstra search to find all connected tiles
            while (openList.Count > 0)
            {
                // Pop off the top
                BoardTile top = openList[0];
                openList.Remove(top);

                // Add self to closed list
                closedList.Add(top);

                foreach(BoardTile neighbor in top.Neighbors)
                {
                    // If the owner doesn't match, skip
                    if(neighbor.Owner != top.Owner) { continue; }

                    // If already on the open list, skip
                    if (openList.Contains(neighbor)) { continue; }

                    // If already on the closed list, skip
                    if (closedList.Contains(neighbor)) { continue; }

                    // Otherwise, add to closed list
                    closedList.Add(neighbor);
                }

                
            }

                return closedList;
        }

        public List<List<BoardTile>> GetAllConnectedTiles(Player player)
        {


            List<List<BoardTile>> listOfConnectedTiles = new List<List<BoardTile>>();

            // Search through indexes for all player tiles
            foreach(BoardTile tile in nodes)
            {
                // Skip if tile is owner
                if(tile.Owner != player) 
                {
                    continue; 
                }

                bool shouldSkip = false;
                // Check if tile is already in the collection
                for(int i =0; i < listOfConnectedTiles.Count; ++i)
                {
                    // Skip adding tile if we already have it.
                    if(listOfConnectedTiles[i].Contains(tile))
                    {
                        shouldSkip = true;
                        break;
                    }
                }
                // Skip if should skip
                if (shouldSkip) { continue; }

                // If they do match, search for other connected tiles and add to collection
                listOfConnectedTiles.Add(GetConnectedTiles(tile.ID));
            }
            // Return the collection
            return listOfConnectedTiles;
        }


    }
    
    [System.Serializable]
    public class Continent
    {
        //---------------------------------- Variables -----------------------------------------
        
        [SerializeField] List<int> tiles;

        [SerializeField] List<BonusBase> bonuses;

        Board board;

        //------------------------------------ Properties --------------------------------------

        public int TileCount
        {
            get
            {
                return tiles.Count;
            }
        }

        //---------------------------------- Public Functions -----------------------------------

        public Continent()
        {
            
        }

        /// <summary>
        /// Sets the board reference for this continent
        /// </summary>
        /// <param name="board"></param>
        public void SetBoard(Board board)
        {
            this.board = board;
        }
        
        /// <summary>
        /// Checks all of the tiles within itself and states if the continent is owned by another 
        /// </summary>
        /// <returns>If all of the tiles are owned by a single user, returns true. Otherwise, returns false.</returns>
        public bool IsOwned()
        {
            // The continent is not owned if it does not exist
            if(TileCount <= 0) { return false; }

            // Get the owner of the first tile of the continent
            Player owner = board[tiles[0]].Owner;

            // Search through the continent to see if everything is owned by the same player
            for(int i= 1; i < TileCount; ++i)
            {
                // If they don't match, then report that it is not owned
                if(owner != board[tiles[i]].Owner) { return false; }
            }

            // If it passes all the above tests, it is owned by one player
            return true;
        }

        /// <summary>
        /// Get the player who owns the continent.
        /// </summary>
        /// <returns>If a player owns all of the continent, then it returns that player. Otherwise, returns null</returns>
        public Player GetOwningPlayer()
        {
            // Check if the continent is owned
            if(!IsOwned())
            {
                // If not owned, return nobody
                return null;
            }

            // 
            return board[tiles[0]].Owner;
        }


    }
}
