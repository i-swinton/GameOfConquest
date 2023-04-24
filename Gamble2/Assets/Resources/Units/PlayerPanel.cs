using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerPanel : MonoBehaviour
{
    private int playerInt;
    private Color playerColor;
    private int TroopCount;
    public TextMeshPro text;

    // Update is called once per frame
    void Update()
    {
        text.text = TroopCount.ToString();
    }

    public void Setup(Player player)
    {
        playerInt = player.playerID;
        playerColor = player.playerColor;
        TroopCount = player.troopCount;
        GetComponent<SpriteRenderer>().color = playerColor;
    }

    public void OnMouseDown()
    {
        GameMaster gm = FindObjectOfType<GameMaster>();

        if (gm.GetPlayerTurn() == playerInt)
        {
            TroopCount++;
        }
        else
        {
            TroopCount--;
        }
    }
}
