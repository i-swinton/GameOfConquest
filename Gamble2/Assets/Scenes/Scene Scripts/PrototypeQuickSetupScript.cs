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

            return;
        }

        GameMaster.GetInstance().StartGame(numOfPlayers);



        IsVisible = false;
    }

    public void NetInit(int numOfPlayers)
    {
        if (GameMaster.HasStarted) { return; }
        GameMaster.GetInstance().StartGame(numOfPlayers);
        IsVisible= true;
    }


   
}
