using System;
using System.Collections;
using System.Collections.Generic;
using Actions;
using TMPro;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    private int turnTacker = 0;
    public int PlayerAmount = 2;
    private List<Player> players = new List<Player>();
    private GameState state;

    public Vector2 BeginningOfUILine;
    public Vector2 EndOfUILine;
    public GameObject playerPanelPrefab;

    public TextMeshProUGUI text;
    
    private MapTile Challenger;
    private MapTile Defender;

    private Actions.ActionList actions;
    // Start is called before the first frame update
    void Start()
    {
        state = GameState.Claim;
        
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

        actions = new ActionList();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Player " + (turnTacker+1).ToString() + "\n" + state.ToString();

        // Update the action list
        if(actions != null)
        {
            actions.Update(Time.deltaTime);
        }
    }
    
    public void SetChallenger(MapTile mt)
    {
        Challenger = mt;
        actions.Add(new Actions.Scale(Vector3.one * 1.4f, Vector3.one, Challenger.gameObject, 0.2f,0.0f, ActionType.NoGroup, false, EaseType.Linear ));
        foreach (MapTile mapTile in FindObjectsOfType<MapTile>())
        {
            if (mt.NodeRef.Neighbors.Contains(mapTile.NodeRef))
            {
                actions.Add(new Actions.Scale(Vector3.one * 1.1f, Vector3.one, mapTile.gameObject, 0.2f,0.0f, ActionType.NoGroup, false, EaseType.Linear ));
            }
        }
    }
    
    public MapTile GetChallenger()
    {
        return Challenger;
    }

    public void ReleaseChallenger()
    {
        if (Challenger != null)
        {
            actions.Add(new Actions.Scale(Vector3.one, Vector3.one * 1.4f, Challenger.gameObject, 0.2f,0.0f, ActionType.NoGroup, false, EaseType.Linear ));
            foreach (MapTile mapTile in FindObjectsOfType<MapTile>())
            {
                if (Challenger.NodeRef.Neighbors.Contains(mapTile.NodeRef))
                {
                    actions.Add(new Actions.Scale(Vector3.one, Vector3.one * 1.1f, mapTile.gameObject, 0.2f,0.0f, ActionType.NoGroup, false, EaseType.Linear ));
                }
            }
            Challenger = null;
        }
    }

    public bool HasChallengerCheck()
    {
        return Challenger != null;
    }
    

    public void EndTurn()
    {
        switch (state) {
            case GameState.Claim:
                if(AllTerroritesClaim())
                    ChangeState(GameState.Reinforce);
                IncrementTurnTracker();
                break;
            case GameState.Reinforce:
                IncrementTurnTracker();
                if(ReinforcingDone())
                    ChangeState(GameState.Draft);
                break;
            case GameState.Draft: ChangeState(GameState.Attack); break;
            case GameState.Attack: ChangeState(GameState.Fortify); break; 
            case GameState.Fortify: IncrementTurnTracker(); ChangeState(GameState.Draft); break;
        }
    }

    bool AllTerroritesClaim()
    {
        foreach (MapTile tile in FindObjectsOfType<MapTile>())
        {
            if (tile.Player == null)
            {
                return false;
            }
        }

        return true;
    }

    bool ReinforcingDone()
    {
        foreach (Player player in players)
        {
            if (player.draftTroop > 0)
                return false;
        }

        return true;
    }

    void IncrementTurnTracker()
    {
        turnTacker++;
        if (turnTacker >= PlayerAmount)
            turnTacker = 0;
    }

    void ChangeState(GameState desiredState)
    {
        if(desiredState == GameState.Draft)
        {
            players[turnTacker].draftTroop += 3;
        }
        state = desiredState;
    }

    public int GetPlayerTurn()
    {
        return turnTacker;
    }

    public GameState GetState()
    {
        return state;
    }

    public Player GetPlayer()
    {
        return players[turnTacker];
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
        draftTroop = 20;
    }

    public int playerID;
    public Color playerColor;
    public int troopCount;
    public int draftTroop;
    public bool isAlive;
}