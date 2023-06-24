using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace MapSystem
{

    public class MapNetData 
    {
        // The ids of the owners of each tile within the game
        List<int> tileOwners;
        // The number of troops on a given tile
        List<int> tileTroops;

        // Total number of players
        int playerCount;

        List<int> numberOfPlayerUnits;

        // The seed value of the rng at the given time.
        int rngSeed;
    }

}