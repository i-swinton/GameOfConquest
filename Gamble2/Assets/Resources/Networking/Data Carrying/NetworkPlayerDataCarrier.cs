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
        data = new NetworkSystem.GameData();
    }
    
    public static List<ClientPlayerController> Controllers
    {
        get
        {
            if(instance.data.cpcs == null)
            {
                instance.data.cpcs = new List<ClientPlayerController>();
            }
            return instance.data.cpcs;
        }
    }

    public static void LoadInPlayer(ClientPlayerController contorller)
    {
        if(instance.data.cpcs == null) { instance.data.cpcs = new List<ClientPlayerController>(); }

        instance.data.cpcs.Add(contorller);

    }



    public static void InitializeGame(GameMaster master)
    {
        // Add in the players
        for(int i=0; i < instance.data.cpcs.Count; ++i)
        {
            GameMaster.AddPlayerController(instance.data.cpcs[i]);
        }

        master.StartGame(instance.data.cpcs.Count,instance.data.settings);

    }

}

namespace NetworkSystem
{
    public class GameData
    {
        public List<ClientPlayerController> cpcs;
        public GameSettings settings;
    }
}
