using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    private int turnTacker = 0;
    public int PlayerAmount = 2;
    private List<Player> players = new List<Player>();

    public Vector2 BeginningOfUILine;
    public Vector2 EndOfUILine;
    public GameObject playerPanelPrefab;

    public TextMeshProUGUI text;
    
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerAmount <= 1)
            PlayerAmount = 2;
        
        for (int i = 0; i < PlayerAmount; i++)
        {
            Player p = new Player();
            p.playerID = i;
            float t = (float) i / PlayerAmount;
            Color color = Color.HSVToRGB(Mathf.Lerp(0.0f, 1.0f, t), 1.0f, 1.0f);
            color.a = 1.0f;
            p.playerColor = color;
            players.Add(p);
            Vector3 pos = Vector2.Lerp(BeginningOfUILine, EndOfUILine, t);
            GameObject panel = Instantiate(playerPanelPrefab, pos, Quaternion.identity);
            panel.GetComponent<PlayerPanel>().Setup(p);
        }
        
        print(players.Count);
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Turn " + turnTacker.ToString();
    }

    public void EndTurn()
    {
        turnTacker++;
        if(turnTacker >= PlayerAmount)
            turnTacker = 0;
    }

    public int GetPlayerTurn()
    {
        return turnTacker;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(BeginningOfUILine,0.5f);
        Gizmos.DrawSphere(EndOfUILine,0.5f);
        Gizmos.DrawLine(BeginningOfUILine, EndOfUILine);
    }
}

public enum GameState{
    Claim,
    Reinforce,
    Draft,
    Attack,
    Fortify
}

public class Player
{
    public Player()
    {
        troopCount = 5;
        isAlive = true;
    }

    public int playerID;
    public Color playerColor;
    public int troopCount;
    public bool isAlive;
}