using System;
using System.Collections;
using System.Collections.Generic;
using Actions;
using TMPro;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public Player Winner { get { return winningPlayer; } }


    private int turnTacker = 0;
    public int PlayerAmount = 2;
    private List<Player> players = new List<Player>();
    private GameState state;

    private Player winningPlayer = null;


    public Vector2 BeginningOfUILine;
    public Vector2 EndOfUILine;
    public GameObject playerPanelPrefab;

    public TextMeshProUGUI text;
    
    private MapTile Challenger;
    private MapTile Defender;

    private Actions.ActionList actions;

    [SerializeField]
    GameMode gameMode;
    MapSystem.Board gameBoard;


    bool hasGameStarted;

    // Singleton
    static GameMaster instance;


    private void Awake()
    {
        instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        hasGameStarted = false;
        actions = new ActionList();
        gameBoard = BoardManager.instance.GetBoard();
    }

    public static GameMaster GetInstance()
    {
        return instance;
    }

    public void StartGame(int numberOfPlayers)
    {
        // Set the game as started
        hasGameStarted = true;
        PlayerAmount = numberOfPlayers;

        state = GameState.Claim;

        if (PlayerAmount <= 1)
            PlayerAmount = 2;

        for (int i = 0; i < PlayerAmount; i++)
        {
            Player p = new Player();
            p.playerID = i;
            float t = (float)i / PlayerAmount;
            Color color = Color.HSVToRGB(Mathf.Lerp(0.0f, 1.0f, t), 1.0f, 1.0f);
            color.a = 1.0f;
            p.playerColor = color;
            players.Add(p);

            PlayerPanelUI.SpawnPlayerPanel(p);

            //Vector3 pos = Vector2.Lerp(BeginningOfUILine, EndOfUILine, t);
            //GameObject panel = Instantiate(playerPanelPrefab, pos, Quaternion.identity);
            //panel.GetComponent<PlayerPanel>().Setup(p);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasGameStarted)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                StartGame(2);

            }

            return;
        }

        if(gameBoard ==null)
        {
            gameBoard = BoardManager.instance.GetBoard();
        }

        text.text = "Player " + (turnTacker+1).ToString() + "\n" + state.ToString();

        // Update the action list
        if(actions != null)
        {
            actions.Update(Time.deltaTime);
        }


        if(Input.GetKeyDown(KeyCode.N))
        {
            EndTurn();
        }

        if( Input.GetKey(KeyCode.B))
        {
            // If the right characters are ready, perform the battle sequence
            if(Defender && Challenger)
            {
                CompleteBattle(-4);
            }
        }

        if (gameMode)
        {
            // Check if the game is over
            if (GetState()!= GameState.Claim && gameMode.CheckIfOver(this, gameBoard))
            {
                PlayWinSequence(gameMode.WinningPlayer(this, gameBoard));
            }
        }else
        {
            Debug.LogWarning("The Game Mode is missing. If you want to win, you are going to need that");
        }

    }
    
    public void SetDefender(MapTile mt)
    {
        Defender = mt;
        // Do whatever actions you want to 

        
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

    public MapTile GetDefender()
    {
        return Defender;
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

            MapDrawSystem.CancelArrow();
        }
    }

    public void ReleaseDefender()
    {
        if(Defender!=null)
        {
            Defender = null;
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

    public Player GetPlayerAt(int index)
    {
        return players[index];
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(BeginningOfUILine,0.5f);
        Gizmos.DrawSphere(EndOfUILine,0.5f);
        Gizmos.DrawLine(BeginningOfUILine, EndOfUILine);
    }

    //------------------------------------------ Combat Functions -------------------------------------------
    public void CompleteBattle(int index =-1)
    {

        int AtkUnitsLost = 0, DefUnitsLost = 0;
        CombatSystem.BattleTiles(GetChallenger().NodeRef, Defender.NodeRef,(Combat.CombatRollType)(-index), out AtkUnitsLost, out DefUnitsLost);

        if (Defender.Units <= 0)
        {

            Defender.SetOwner(GetChallenger().Player);

            // Pull up the confirm for fortify
            ConfirmUI.BeginConfirm("Fortify", ConfirmUI.ConfirmType.Fortify,
                GetChallenger().NodeRef, GetDefender().NodeRef, Mathf.Clamp((-index), 1, 3));
            //Defender.NodeRef.Fortify(3);

        }
        else
        {

            // Perform the proper cleanup
            ReleaseChallenger();
            ReleaseDefender();
            MapDrawSystem.CancelArrow();
        }
    }

    // ------------------------------------------ Fortify Functions ----------------------------------------------

    public void CompleteFortify(int amount)
    {
        // Transfer over the units
        Defender.NodeRef.TransferUnits( Challenger.NodeRef, amount);

        ReleaseChallenger();
        ReleaseDefender();
        MapDrawSystem.CancelArrow();

        // Force continue after you fortify
        if(GetState() == GameState.Fortify)
        {
            EndTurn();
        }
    }

    // ------------------------------------------ Confirm Functions --------------------------------------------
    public void Confirm(int value)
    {
        switch(GetState())
        {
            // Handle combat confirmation
            case GameState.Attack:
                {
                    if (value <= 0)
                    {
                        // NOTE: Will want to make fortify changes here as well.
                        CompleteBattle(value);
                    }
                    else
                    {
                        CompleteFortify(value);
                    }

                    break;
                }
            case GameState.Fortify:
                {
                    CompleteFortify(value);
                    break;
                }
                
        }
    }

    // ------------------------------------------- Win Functions ----------------------------------------------
    public void PlayWinSequence(Player winner)
    {
        // Set the winner
        winningPlayer = winner;
        // Display Win Text
        WinLossUI.Open("Victory!", true, winner);
        // Shut everything down
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

    public string Name
    {
        get
        {
            return "Player " + playerID;
        }
    }

    string name;
    public int playerID;
    public Color playerColor;
    public int troopCount;
    public int draftTroop;
    public bool isAlive;
    public List<TerritoryCard> cards;
    // Insert spot for player UI
}