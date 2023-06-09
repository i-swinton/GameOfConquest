using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RPC_Test : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsServer && IsOwner) //Only send an RPC to the server on the client that owns the NetworkObject that owns this NetworkBehaviour instance
        {
            //TestServerRpc(0, NetworkObjectId);
        }
    }

    [ClientRpc]
    void TestClientRpc(int value, ulong sourceNetworkObjectId)
    {
        DebugNetworklLog.Log($"Client Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
        //NetworkConsole.instance.SetText($"Server Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
        if (IsOwner) //Only send an RPC to the server on the client that owns the NetworkObject that owns this NetworkBehaviour instance
        {
            TestServerRpc(value + 1, sourceNetworkObjectId);
        }
    }

    
    [ServerRpc]
    void TestServerRpc(int value, ulong sourceNetworkObjectId)
    {
        DebugNetworklLog.Log($"Server Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
        //NetworkConsole.instance.SetText($"Server Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
        TestClientRpc(value, sourceNetworkObjectId);
    }
}
