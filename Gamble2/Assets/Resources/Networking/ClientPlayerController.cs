using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ClientPlayerController : NetworkBehaviour
{
    // The identity of the current player
    Player player;

    static ClientPlayerController instance;

    static int id = 0;

    int currentID = 0;

    public int InternalID
    {
        get
        {
            return currentID;
        }
    }

    public Player Player
    {
        get { return player; }
    }
public static ClientPlayerController Instance
    { get { return instance; } }

    public static bool IsCurrentPlayer(GameMaster master)
    {
        return Instance.Player.playerID == GameMaster.GetInstance().GetPlayer().playerID;
    }

    public static int LocalPlayer
    {
        get
        {
            return Instance.Player.playerID;
        }
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Set the current ID, and increment the id
        currentID = id;
        id++;
        
        // Mark the Player Controller on the user side
        if(IsOwner)
        {
            instance= this;
            DebugNetworklLog.SetTitle(IsHost ? "Host" : "Client");
        }

        if (GameMaster.GetInstance() != null)
        {
            GameMaster.AddPlayerController(this);
        }
        else
        {
            NetworkPlayerDataCarrier.LoadInPlayer(this);
        }
      

        DebugNetworklLog.Log("Spawning Player for "+(IsHost? "Host":"Client")+": " + name);

    }

    public void SetPlayerIdentity(Player player)
    {
        this.player = player;

        DebugNetworklLog.Log("Player Connected: " + player.Name);

        if (IsOwner)
        {
            DebugNetworklLog.SetTitle((IsHost ? "Host" : "Client") + ": " + player.playerID);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
