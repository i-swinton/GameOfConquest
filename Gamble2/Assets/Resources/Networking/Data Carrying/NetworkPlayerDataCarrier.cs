using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerDataCarrier : MonoBehaviour
{
    static NetworkPlayerDataCarrier instance;
    NetworkSystem.GameData data;
    

    private void Awake()
    {
        instance = this;
    }
    
    public static void LoadInPlayer(ClientPlayerController contorller)
    {
        if(instance.data.cpcs == null) { instance.data.cpcs = new List<ClientPlayerController>(); }

        instance.data.cpcs.Add(contorller);

    }

}

namespace NetworkSystem
{
    public class GameData
    {
        public List<ClientPlayerController> cpcs;
    }
}
