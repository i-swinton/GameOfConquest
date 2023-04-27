using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeQuickSetupScript : UIElement
{

    public void QuickInit(int numOfPlayers)
    {
        GameMaster.GetInstance().StartGame(numOfPlayers);

        IsVisible = false;
    }


   
}
