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
    public void SyncRNG_ServerRPC(int seed)
    {
        SyncRNG_ClientRPC(seed);
    }

    [ClientRpc]
    public void SyncRNG_ClientRPC(int seed)
    {
        RNG.SeedRandom(seed);
    }

    [ServerRpc]
    public void ConfirmBattle_ServerRPC(int value)
    {
        GameMaster.GetInstance().ConfirmBattle_ClientRPC(value);
    }

    [ServerRpc]
    public void ConfirmFortify_ServerRPC(int value)
    {
        GameMaster.GetInstance().ConfirmFortify_ClientRPC(value);
    }

    [ServerRpc]
    public void AddBot_ServerRPC()
    {
        NetworkLobbyScript.Instance.AddBot_ClientRPC();
    }

    [ServerRpc]
    public void ReadyComplete_ServerRPC()
    {

        GameMaster.GetInstance().readyCount.Value += 1;
        DebugNetworklLog.Log($"{GameMaster.GetInstance().readyCount.Value} vs {GameMaster.GetInstance().HumanPlayers}");
        if (GameMaster.GetInstance().readyCount.Value == GameMaster.GetInstance().HumanPlayers)
        {
            Invoke("ReadyCompleteInvokeCall", 1.0f);
        }

    }

    public void ReadyCompleteInvokeCall()
    {
        GameMaster.GetInstance().ReadyComplete_ClientRPC();

    }




    [ServerRpc]
    public void SyncBoards_ServerRPC(int tileId, int ownerID, int numberOfUnits)
    {
        GameMaster.GetInstance().SyncBoardTile_ClientRPC(tileId, ownerID, numberOfUnits);

    }

    [ServerRpc]
    public void SyncPlayer_ServerRPC(int playerID, int troopCount)
    {
        GameMaster.GetInstance().SyncPlayer_ClientRPC(playerID, troopCount);
    }

    //    // tileOwners
    //    // tileTroops
    //    // playerCount
    //    // num of players
    //    // rng seed
    [ServerRpc]
    public void SyncBoardsFull_ServerRPC(int[] tileOwners, int[] tileTroops, int playerCount, int[] numOfTroops, int rngSeed)
    {
        DebugNetworklLog.Log("Sending Sync Request");
        GameMaster.GetInstance().SyncBoardFull_ClientRPC(tileOwners, tileTroops, playerCount, numOfTroops, rngSeed);
    }

}
