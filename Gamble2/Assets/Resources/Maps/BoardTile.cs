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

        Unit units;

        Board parent;

        List<BonusBase> bonuses;

        string name;

        int tileGroup;

        // Actions

        public System.Action onSelect;
        public System.Action onDeselect;
        public System.Action<bool> onSelectable;
        public System.Action onUnselectable;

        public System.Action<BonusBase> onBindBonus;

        public System.Action<bool> onVisibleUpdate;

        //--------------------------------------------- Properties---------------------------------------------------

        /// <summary>
        /// The location of this tile in world space.
        /// </summary>
        public Vector3 Position 
        {
            get
            {
                return worldPosition;
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
        /// The number of units on this tile
        /// </summary>
        public int UnitCount
        {
            get
            {
                return units.Count;
            }
        }

        /// <summary>
        /// The player which currently has this tile claimed.
        /// </summary>
        public Player Owner
        {
            get
            {
                return owner;
            }
        }

        /// <summary>
        /// The board which this tile is a part of.
        /// </summary>
        public Board Parent
        {
            get
            {
                return parent;
            }
        }

        /// <summary>
        /// A list of all the bonuses being applied to the tile.
        /// </summary>
        public List<BonusBase> Bonuses
        {
            get
            {
                return bonuses;
            }
        }

        public int TileGroup
        {
            get
            {
                return tileGroup;
            }
            set
            {
                // Make sure it is a valid tile group id
                if (value > -1)
                {
                    tileGroup = value;
                }
            }
        }
       

        // -------------------------------------------- Public Functions ---------------------------------------------
        public BoardTile(int id, Vector3 worldPosition, Board parent, string name, int tileGroup ): base(id)
        {

            this.worldPosition = worldPosition;
            this.bonuses = new List<BonusBase>();
            this.parent = parent;
            this.name = name;

            this.tileGroup = tileGroup;
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
            // Add the units
            if (owner != null) { owner.OnTroopCountChange(newUnits.Count); }
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
            if(owner!=null)
            {
                owner.OnTroopCountChange(additionalUnits);
            }
        }
        


        public void TransferUnits(ref Unit otherUnits, int amount)
        {
            // Subtract from the other units
            otherUnits -= amount;

            // Add to the current amount
            units += amount;
        }

        public void TransferUnits(BoardTile other, int amount)
        {
            TransferUnits(ref other.units, amount);
        }

        /// <summary>
        /// Kills units on the board tile
        /// </summary>
        /// <param name="amount"></param>
        public void KillUnits(int amount)
        {
            // Only if we have owner
            if (owner != null)
            {
                owner.OnTroopCountChange(-amount);
            }
            units -= amount;
        }

        /// <summary>
        /// Change the owner of the of tile.
        /// </summary>
        /// <param name="player">The new owner of the board tile.</param>
        public void ChangeOwner(Player player)
        {
            // Ignore if the players match
            if(owner == player) { return; }
            
         

            if(owner != null)
            {

                Debug.Log($"Changed ownership from {owner} to {player}");

                owner.OnTileCountChange(-1);

                //owner.territoryCount -= 1;
                // On Tile change
                owner.ChangeTileOwner(this);
            }

            // Change the owner to the new player
            owner = player;


            player.ChangeTileOwner(this);
            owner.OnTileCountChange(1);
            //player.territoryCount += 1;


            VisibleUpdate();
            // Update neighbors
            foreach(BoardTile tile in Neighbors)
            {
                tile.VisibleUpdate();
            }
        }

        /// <summary>
        /// Adds a bonus to the list of bonuses on this tile.
        /// </summary>
        /// <param name="bonus">The bonus being added to the tile.</param>
        public void ApplyBonus(BonusBase bonus)
        {
            // Add a bonus to the list
            bonuses.Add(bonus);

            onBindBonus?.Invoke(bonus);
        }

        public void Select()
        {
            onSelect?.Invoke();
        }

        public void Deselect()
        {
            onDeselect?.Invoke();
        }

        public void VisibleUpdate()
        {
            GameMaster gm = GameMaster.GetInstance();

            if (!gm.InFogOfWar) { return; }
            
            // Network check
            if((gm.IsNetworked))
            {
                if (Owner != null)
                {
                    if (Owner.playerID == ClientPlayerController.LocalPlayer)
                    {
                        onVisibleUpdate?.Invoke(true);
                        return;
                    }
                    else
                    {
                        foreach (BoardTile tile in Neighbors)
                        {
                            if (tile.Owner != null &&tile.Owner.playerID == ClientPlayerController.LocalPlayer)
                            {
                                if (gm.GetState() > GameState.Reinforce)
                                {
                                    onVisibleUpdate?.Invoke(true);
                                    return;
                                }
                            }
                        }
                    }


                }
                else
                {
                    // Show all unclaimed tiles
                    onVisibleUpdate?.Invoke(true);
                    return;
                }
            } // Nonetwork check
            else if(!gm.IsNetworked )
            {
                if(Owner != null)
                {
                    if (Owner.playerID == 0)
                    {
                        Debug.Log("Owned By player");

                    }
                    if (Owner == gm.CurrentPlayer)
                    {
                        onVisibleUpdate?.Invoke(Owner.isHuman);
                        return;
                    }
                    else
                    {
                        foreach (BoardTile tile in Neighbors)
                        {
                            if(tile.Owner != null && tile.Owner.playerID ==0)
                            {
                                Debug.Log("Owned By player");
                               
                            }
                            if (tile.Owner == gm.CurrentPlayer 
                                && gm.GetState() > GameState.Reinforce) // Makes Claim and Reinforce hidden
                            {
                               
                                onVisibleUpdate?.Invoke(true);
                                return;
                            }

                            // If empty tile, don't hide it
                            //if(tile.Owner == null)
                            //{
                            //    onVisibleUpdate?.Invoke(true);
                            //    return;
                            //}
                        }
                    }
                }
                else
                {
                    onVisibleUpdate(true);
                    //Selectable(true); // Set the tile to be selectables
                    return;
                }
            }

            // Calculate the value
            onVisibleUpdate?.Invoke(false);
        }

        /// <summary>
        /// Sets the tile to be selectable
        /// </summary>
        /// <param name="otherSeleced">True when something can be selected and something is already selected</param>
        public void Selectable(bool otherSeleced)
        {
            onSelectable?.Invoke(otherSeleced);
        }

        public void Unselectable()
        {
            onUnselectable?.Invoke();
        }

        
        /// <summary>
        /// Get a bonus of a given type on the board tile. 
        /// </summary>
        /// <param name="bType">The type of bonus on the board tile.</param>
        /// <returns>If the bonus of that type exists, returns </returns>
        public BonusBase GetBonusOfType(BonusBase.BonusType bType)
        {
            for(int i =0; i < bonuses.Count; ++i)
            {
                if(bonuses[i].MyType == bType)
                {
                    return bonuses[i];
                }
            }

            // If there is no bonus in that base, return null
            return null;
        }

        public override string ToString()
        {
            return $"{Name}";
        }

        // ---------------------------------------------- DEBUG FUNCTIONS --------------------------------------------
        public void DrawConnections()
        {
            // Draw the line to each node
            foreach(BoardTile node in Neighbors)
            {
                Gizmos.DrawLine(worldPosition, node.Position);
            }
        }
    }
}
