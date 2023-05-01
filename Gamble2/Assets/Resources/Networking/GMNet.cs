using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
public class GMNet : NetworkBehaviour
{
    static GMNet instance;

    public static GMNet Instance
    {
        get
        {
            return instance;
        }
    }

    // Set the instance
    public override void OnNetworkSpawn()
    {
        
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            instance = this;
        }
    }
    
    [ServerRpc]
    public void OnMapTileClickServerRPC(int nodeID, int mapTileID)
    {
        // Perform the action on all instances of the game
        GameMaster.GetInstance().OnTileClickClientRPC(nodeID, mapTileID);
    }
}
