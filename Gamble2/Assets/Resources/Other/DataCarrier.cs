using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCarrier : MonoBehaviour
{

    static DataCarrier instance;

    

    NetworkSystem.GameData data;

    int playerCount;
    int botCount;

    public System.Action onLoadInGame;

    public static DataCarrier Instance
    {
        get { return instance; }
    }

    public static int PlayerCount
    {
        get { return instance.playerCount; }
    }
    public static int BotCount
    {
        get { return instance.botCount; }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            data = new NetworkSystem.GameData();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        if (instance == this)
        {
            // Don't destroy with level loading
            DontDestroyOnLoad(this);
        }
    }

    public static void AddPlayer()
    {
        instance.playerCount++;
    }

    public static void DestroyCarrier()
    {
        Destroy(instance.gameObject);
    }


    public static void RemovePlayer()
    {
        if(instance.playerCount <=0)
        {
            return;
        }

        --instance.playerCount;
    }

    public static void AddBot()
    {


        ++instance.botCount;
    }

    public static void RemoveBot()
    {
        if(instance.botCount <= 0)
        {
            return;
        }

        --instance.botCount;

    }


    public static void LoadGameData(GameMode mode, GameSettings settings, MapData map)
    {
        // Set the settings to the given value
        instance.data.settings = settings;

        // Set the game mode
        instance.data.mode = mode;

        // Set the game map
        instance.data.map = map;

        instance.onLoadInGame?.Invoke();
    }

    public static void InitializeGame(GameMaster master)
    {
        //  Check instance
        if (instance == null) return;
        // Add in the players
        //for (int i = 0; i < instance.data.cpcs.Count; ++i)
        //{
        //    GameMaster.AddPlayerController(instance.data.cpcs[i]);
        //}
        Debug.Log("Settings are: " + (instance.data.settings != null ? "Exists" : "Null"));

        //NOTE: Add a computer player count insert here as well
        master.StartGame(instance.playerCount, new AI.AIPlayerData(instance.botCount), instance.data.settings, instance.data.mode);

    }

    public static MapData GetMap()
    {
        return instance.data.map;
    }
}
