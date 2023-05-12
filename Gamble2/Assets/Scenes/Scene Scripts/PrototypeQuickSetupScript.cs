using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeQuickSetupScript : UIElement
{
    public static PrototypeQuickSetupScript instance;

    private void Awake()
    {
        instance = this;
    }

    public void QuickInit(int numOfPlayers)
    {
        if (GameMaster.GetInstance().IsNetworked)
        {
            GameMaster.GetInstance().StartGameDebugServerRPC(numOfPlayers);
            return;
        }

        AI.AIPlayerData adp = new AI.AIPlayerData(0);
        if(numOfPlayers == 3) { numOfPlayers = 0;  adp = new(3); }
        GameMaster.GetInstance().StartGame(numOfPlayers, adp, null, null) ;



        IsVisible = false;
    }

    public void NetInit(int numOfPlayers)
    {
        if (GameMaster.HasStarted) { return; }
        GameMaster.GetInstance().StartGame(numOfPlayers);
        IsVisible= false;
    }


   
}
