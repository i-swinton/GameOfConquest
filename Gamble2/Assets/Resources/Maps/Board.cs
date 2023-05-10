using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapSystem
{

    
    public class Board:Graph
    {

        // Used to hold game relevant information about the graph. 
        // May want to have it also hold a disjoint graph to quickly check if two board tiles are connected, but that is a later issue

        // --------------------------------------- Variables ------------------------------------------------------
        [Tooltip("The list of continents on the board")]
        [SerializeField] List<Continent> continents;

        int searchIndex=0;
        // ----------------------------------------- Properties ---------------------------------------------------

        

        public int ContinentCount
        {
            get
            {
                return continents.Count;
            }
        }

        // ------------------------------------ Public Functions ---------------------------------------------------

        public Board() : base()
        {
            // Create a new list of continents
            continents = new List<Continent>();
        }

        public Board(List<MapContinent> newCons) : base()
        {
            // Create a new list of continents
            continents = new List<Continent>();
            int conIDs = 0;
            // Create a number of nodes based on the continents
            foreach (MapContinent con in newCons)
            {

                // Build all of the tiles for the continent
                List<int> conTiles = new List<int>();
                foreach (TileData data in con.Tiles)
                {
                    // Make the node
                    BoardTile tile = MakeNode(data.Name);
                    conTiles.Add(tile.ID);
                }

                // Add the tiles to the continent
                Continent continent = new Continent(conTiles, con.Name, con.bonus, conIDs);
                ++conIDs;
                // Add the continent to the list
                AddContinent(continent);


            }
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
            c.SetBoard(this);
        }

        public Continent FindContinent(string continentName)
        {
            // Search the list for the matching continent
            for(int i =0; i < continents.Count; ++i)
            {
                if(continents[i].Name == continentName)
                {
                    return continents[i];
                }
            }

#if UNITY_EDITOR
            throw new ContinentNotFound("Unable to find continent of name " + continentName);
#else
            return null;
#endif
        }

        /// <summary>
        /// Searches for a continent within the list of continents.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>If the continent is found, returns that continent. Otherwise, returns null.</returns>
        public Continent FindContinent(int index)
        {
            if(index >= continents.Count)
            {
#if UNITY_EDITOR
                throw new ContinentNotFound("Unable to find continent at index " + index);
#else
                return null;
#endif
            }

            return continents[index];
        }



        public List<BoardTile> FindPathTo(BoardTile startTile, Continent continent)
        {
            // Use a dijsktra flood search to find the first tile which is of the continent
            List<BoardTile> openList = new List<BoardTile>();
            List<BoardTile> closedList = new List<BoardTile>();

            List<BoardTile> path = new List<BoardTile>();

            startTile.prior = null;
            openList.Add(startTile);

            while (openList.Count > 0)
            {
                // Pop the first node off the list
                var current = openList[0];
                openList.RemoveAt(0);

                // Add to the closed list
                closedList.Add(current);


                // If the current node is in the continent, return
                if(continent.Contains(current))
                {
                    while (current != null)
                    {
                        path.Insert(0,current);

                        current = (BoardTile)current.prior;
                    }

                    if (searchIndex == int.MaxValue) { searchIndex = 0; }
                    else { ++searchIndex; }

                    // Return the list
                    return path;
                }

                // Fill the open list with the neighbors
                foreach (BoardTile neighbor in current.Neighbors)
                {
                   if(openList.Contains(neighbor))
                    {
                        if(neighbor.g > current.g && neighbor.searchIndex == searchIndex)
                        {
                            neighbor.g = current.g + 1;
                            neighbor.searchIndex = searchIndex;
                            neighbor.prior = current;
                        }
                    }
                   else if (closedList.Contains(neighbor))
                    {
                        // Skip if on the closed list
                    }
                   else // If not on anything else, add to open list
                    {
                        openList.Add(neighbor);
                        neighbor.g = current.g + 1;
                        neighbor.searchIndex = searchIndex;
                        neighbor.prior = current;
                    }
                }
            }

            if (searchIndex == int.MaxValue) { searchIndex = 0; }
            else { ++searchIndex; }

            // Return the empty path
            return path;
        }

        //---------------------------------------- Overridden Functions --------
        //
        //------------------------------------
        /// <summary>
        /// Adds a boardTile to the board.
        /// </summary>
        /// <returns>Returns a reference to the board.</returns>
        public override Node MakeNode()
        {
            return MakeNode(Vector3.zero, "Unnamed Board");
        }

        public BoardTile MakeNode(Vector3 worldPosition)
        {
            return MakeNode(worldPosition, "Unnamed Board");
        }

        public BoardTile MakeNode(Vector3 worldPosition, string name)
        {
            return MakeNode(worldPosition, name, -1);
        }

        public BoardTile MakeNode(Vector3 worldPosition, string name, int tileGroup)
        {
            // Create new node and increment the next index
            BoardTile node = new BoardTile(nextNodeIndex++, worldPosition, this, name, tileGroup);

            // Add the new node to the list
            nodes.Add(node);
            return node;
        }
        public BoardTile MakeNode(string name, int tileGroup)
        {
            return MakeNode(Vector3.zero, name, tileGroup);
        }

        public BoardTile MakeNode(string name)
        {
            return MakeNode(Vector3.zero, name);
        }
       
        /// <summary>
        /// Sees if the node with the given name exists within the board
        /// </summary>
        /// <param name="name">The name of the target node.</param>
        /// <returns>If the node is found, returns true. Otherwise, reutrns false.</returns>
        public bool Contains(string name)
        {
            return PrivateFind(name) != null;
        }

        public BoardTile Find(string name)
        {
            BoardTile tile = (BoardTile)PrivateFind(name);

            if(tile != null)
            {
                return tile;
            }

#if UNITY_EDITOR
            throw new NodeNotFoundException("Board Tile named " + name + " was not found.");
#else
            return null;
#endif
        }

        /// <summary>
        /// Finds a board tile using the name of the tile.
        /// </summary>
        /// <param name="name">The name of the given tile.</param>
        /// <returns>If the tile is found, returns a reference to that tile. Otherwise, returns null.</returns>
        public BoardTile this[string name] => Find(name);

        /// <summary>
        /// Gets a list of all tiles connected to the given node that share a player.
        /// </summary>
        /// <param name="tile">The tile being checked for all connected nodes.</param>
        /// <returns>Returns a list of nodes which are all disjoint to one another by having the same player. </returns>
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
                    //closedList.Add(neighbor);
                    // And add it to the open list
                    openList.Add(neighbor);
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

        public BoardTile GetRandomTile(Player player)
        {
            int index = RNG.Roll(0, Count-1,false);
            
            for(int i=0; i < Count; ++i)
            {
                // If this random tile matches the player passed in, return it
                if(this[index].Owner == player) { return this[index]; }

                // Step the index
                ++index;
                index %= Count;
            }

            // Return nothing if no random tile could be found
            return null;
        }
    }
    
    [System.Serializable]
    public class Continent
    {
        //---------------------------------- Variables -----------------------------------------
        
        [SerializeField] List<int> tiles;

        [SerializeField] List<BonusBase> bonuses;

        Board board;

        [SerializeField] string name;


        Player lastOwningPlayer;

        int id;
        //------------------------------------ Properties --------------------------------------

        public int ID
        {
            get { return id; }
        }

        public int TileCount
        {
            get
            {
                return tiles.Count;
            }
        }
        public List<int> Tiles
        {
            get
            {
                return tiles;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// The player who had owned the continent when last checked.
        /// </summary>
        public Player LastOwningPlayer
        {
            get
            {
                return lastOwningPlayer;
            }
        }

        public List<BonusBase> AllBonuses
        {
            get
            {
                return bonuses;
            }
        }

        public bool IsFull
        {
            get
            {
                for(int i=0; i < TileCount; ++i)
                {
                    // If any of these are empty, return false
                    if(board[tiles[i]].Owner == null)
                    {
                        return false;
                    }
                }

                // If we didn't hit any obstacles, then all spots should be full
                return true;
            }
        }

        //---------------------------------- Public Functions -----------------------------------

        public Continent()
        {
            
        }

        public Continent(List<int> tiles, string name)
        {
            this.tiles = tiles;
            this.name = name;
        }

        public Continent(List<int> tiles, string name, List<BonusBase> bonuses, int _id)
        {
            this.tiles = tiles;
            this.name = name;

            this.bonuses = bonuses;
            this.id = _id;
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
                // If we had a owning player
                if(lastOwningPlayer != null)
                {
                    lastOwningPlayer.OnContinentDisown(this);
                }

                lastOwningPlayer = null;

                // If not owned, return nobody
                return null;
            }
            // Null check
            if (board[tiles[0]].Owner ==null)
            {
                return null;
            }

            lastOwningPlayer = board[tiles[0]].Owner;
            // Update the last owning player

            lastOwningPlayer.OnContinentOwn(this);

            return board[tiles[0]].Owner;
        }

        public bool Contains(BoardTile boardTile)
        {
            return tiles.Contains(boardTile.ID);
        }

        public BoardTile GetRandomTile(Player ownerType)
        {
            // Count -1
            int index = RNG.Roll(0, tiles.Count - 1);

            BoardTile found = null;

            for(int i=0; i < tiles.Count; ++i)
            {
                // If the owners match, set found tile
                if(board[tiles[index]].Owner == ownerType)
                {
                    found = board[tiles[index]];
                    break;
                }

                // Otherwise, increment index within bounds
                ++index;
                index %= tiles.Count;

            }

            // Return the found tile
            return found;
        }

    }

    //--------------------------------------------------- Errors --------------------------------------------------------------
#region Exceptions
    [System.Serializable]
    public class ContinentNotFound : System.Exception
    {


        public ContinentNotFound() { }

        public ContinentNotFound(string message)
            : base(message) { }

        public ContinentNotFound(string message, System.Exception inner)
            : base(message, inner) { }



    }
#endregion
}
