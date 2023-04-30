using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ClientPlayerController : NetworkBehaviour
{
    // The identity of the current player
    Player player;

    static ClientPlayerController instance;


    public Player Player
    {
        get { return player; }
    }
public static ClientPlayerController Instance
    { get { return instance; } }

    public static bool IsCurrentPlayer(GameMaster master)
    {
        return Instance.Player.playerID != GameMaster.GetInstance().GetPlayer().playerID;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Mark the Player Controller on the user side
        if(IsOwner)
        {
            instance= this;
            DebugNetworklLog.SetTitle(IsClient ? "Client" : "Host");
        }
        

        GameMaster.AddPlayerController(this);

        DebugNetworklLog.Log("Spawning Player: " + name);

    }

    public void SetPlayerIdentity(Player player)
    {
        this.player = player;

        DebugNetworklLog.Log("Player Connected: " + player.Name);
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
