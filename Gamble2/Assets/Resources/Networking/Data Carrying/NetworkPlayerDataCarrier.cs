using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerDataCarrier : MonoBehaviour
{
    static NetworkPlayerDataCarrier instance;
    [SerializeField]NetworkSystem.GameData data;
    

    private void Awake()
    {
        instance = this;
        //data = new NetworkSystem.GameData();

        
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

    public static void LoadGameData(GameMode mode, GameSettings settings)
    {
        // Set the settings to the given value
        instance.data.settings = settings;

        // Set the game mode
        instance.data.mode = mode;
    }


    public static void InitializeGame(GameMaster master)
    {
        // Add in the players
        for(int i=0; i < instance.data.cpcs.Count; ++i)
        {
            GameMaster.AddPlayerController(instance.data.cpcs[i]);
        }
        Debug.Log("Settings are: " + (instance.data.settings != null?"Exists":"Null"));
        master.StartGame(instance.data.cpcs.Count,instance.data.settings, instance.data.mode);

    }

}

namespace NetworkSystem
{
    [System.Serializable]
    public class GameData
    {
        public List<ClientPlayerController> cpcs;
        public GameSettings settings;
        public GameMode mode;
    }
}
