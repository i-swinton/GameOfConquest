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

    [ServerRpc]
    public void OnPerformCardInServerRPC(TerritoryCard[] cards, int playerInt)
    {
        GameMaster.GetInstance().OnPerformCardIn_ClientRPC(cards, playerInt);
    }

    [ServerRpc]
    public void EndTurn_ServerRPC()
    {
        GameMaster.GetInstance().EndTurn_ClientRPC();
    }

    [ServerRpc]
    public void ConfirmBattle_ServerRPC(int value)
    {

    }

    [ServerRpc]
    public void ConfirmFortify_ServerRPC(int value)
    {

    }
}
