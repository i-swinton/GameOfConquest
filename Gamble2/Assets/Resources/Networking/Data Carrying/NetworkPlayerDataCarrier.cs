using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class NetworkPlayerDataCarrier : MonoBehaviour
{
    static NetworkPlayerDataCarrier instance;
    [SerializeField]NetworkSystem.GameData data;

    int botCount;

    public System.Action onLoadInGame;

    public static System.Action OnLoadInGame;



    public static int BotCount
    {
        get
        {
            return instance.botCount;
        }
    }


    public static MapData Map
    {
        get
        {
            return instance.data.map;
        }
    }

    private void Awake()
    {
        instance = this;
        //data = new NetworkSystem.GameData();

        
    }

    public static NetworkPlayerDataCarrier Instance
    {
        get
        {
            return instance;
        }
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

    public static void LoadGameData(GameMode mode, GameSettings settings, MapData map)
    {
        // Set the settings to the given value
        instance.data.settings = settings;

        // Set the game mode
        instance.data.mode = mode;

        // Set the map data
        instance.data.map = map;

        instance.onLoadInGame?.Invoke();


    }




    public static void InitializeGame(GameMaster master)
    {
        //  Check instance
        if (instance == null) return;
        // Add in the players
        for(int i=0; i < instance.data.cpcs.Count; ++i)
        {
            GameMaster.AddPlayerController(instance.data.cpcs[i]);
        }
        Debug.Log("Settings are: " + (instance.data.settings != null?"Exists":"Null"));

        //NOTE: Add a computer player count insert here as well
        master.StartGame(instance.data.cpcs.Count,new AI.AIPlayerData(BotCount),instance.data.settings, instance.data.mode);

    }

    public static void AddBot()
    {
        instance.botCount++;
    }

}

// Tells the Netcode how to serialize and deserialize Url in the future.
// The class name doesn't matter here.
public static class SerializationExtensions
{
    public static void ReadValueSafe(this FastBufferReader reader, out NetworkSystem.GameSettingStruct gss)
    {
        reader.ReadValueSafe(out int val);


        //gss = new NetworkSystem.GameSettingStruct();

        gss = new NetworkSystem.GameSettingStruct(val);
    }

    public static void WriteValueSafe(this FastBufferWriter writer, in NetworkSystem.GameSettingStruct gss)
    {
        // Write value safe
        writer.WriteValueSafe(gss.ToInt());
    }

    // Map Net Data
    #region Map Net Data

    //public static void ReadValueSafe(this FastBufferReader reader, out MapSystem.MapNetData map)
    //{
    //    // tileOwners
    //    // tileTroops
    //    // playerCount
    //    // num of players
    //    // rng seed

    //    reader.ReadValueSafe(out List<int> tileOwners);
    //}

    #endregion

}

namespace NetworkSystem
{
    [System.Serializable]
    public class GameData
    {
        public List<ClientPlayerController> cpcs;
        public GameSettings settings;
        public GameMode mode;
        public MapData map;
    }



}

public static class GameType
{
    public static bool isNetworked;
}