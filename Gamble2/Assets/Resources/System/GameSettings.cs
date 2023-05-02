using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSettings 
{
    public bool AutoFillTiles = false;
    public bool AutoReinforce = false;

    public GameSettings()
    {
        Debug.Log("Writing game settings");
    }
}
