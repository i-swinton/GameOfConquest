using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkCleanup : MonoBehaviour
{
    static NetworkCleanup instance;

    /// <summary>
    /// Cleans up the network manager in the non-networked scenes
    /// </summary>
    public static void CleanupNetwork()
    {
        instance.Cleanup();
    }

    private void Awake()
    {
        instance = this;
    }


    /// <summary>
    /// Cleans up the network manager in the non-networked scenes
    /// </summary>
    void Cleanup()
    {
        if(NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }
    }
}
