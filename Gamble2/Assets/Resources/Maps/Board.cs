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
    
    [System.Serializable]
    public class Continent
    {
        //---------------------------------- Variables -----------------------------------------
        
        [SerializeField] List<int> tiles;

        [SerializeField] List<BonusBase> bonuses;


        //---------------------------------- Public Functions -----------------------------------

        public Continent()
        {
            
        }
        
        /// <summary>
        /// Checks all of the tiles within itself and states if the continent is owned by another 
        /// </summary>
        /// <returns>If all of the tiles are owned by a single user, returns true. Otherwise, returns false.</returns>
        public bool IsOwned()
        {
            // 

            return false;
        }

    }
}
