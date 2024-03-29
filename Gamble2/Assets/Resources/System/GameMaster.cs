using System;
using System.Collections;
using System.Collections.Generic;
using Actions;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class GameMaster : NetworkBehaviour
{
    [Header("References")]
    [SerializeField]
    UnityEngine.UI.Button endTurnButton;

    public Player Winner { get { return winningPlayer; } }



    Player lastDeadPlayer;

    private int turnTacker = 0;
    int lastHumanIndex = 0;

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

    [Header("Game Settings")]
    [SerializeField]
    GameMode gameMode;
    MapSystem.Board gameBoard;

    [SerializeField]
    GameSettings settings;

    bool hasGameStarted;
    bool isReady;

    // Singleton
    static GameMaster instance;

    // AI State Management
    bool isInBattle;

    // Networking Variables
    bool isNetworked;
    // Use to check if all players are ready before starting game
    public NetworkVariable<int> readyCount = new NetworkVariable<int>(0);


    public bool IsNetworked { get { return isNetworked; } }



    List<ClientPlayerController> playerControllers;

    //----------------------------------------- Actions -------------------------------------------------
    public System.Action<int> onTurnBegin;

    public System.Action<int, int> onAfterAttack;

    // ------------------------------------ Properties -----------------------------------------------------
    public bool IsInBattle { get { return isInBattle; } }
    public static bool HasStarted
    {
        get
        {
            return instance.hasGameStarted;
        }
    }

    public static bool IsAITurn
    {
        get
        {
            return !instance.GetPlayer().isHuman;
        }
    }

    public bool InFogOfWar { get { return settings.FogOfWar; } }

    public Player CurrentPlayer { get { return players[turnTacker]; } }

    public int HumanPlayers 
    {
        get
        {
            if(IsNetworked)
            {
                return playerControllers.Count;
            }
            else
            {
                return players.Count;
            }
        } 
    }

    // ------------------------------------ Static Functions ---------------------------------------------------


    /// <summary>
    /// Add an action to the central action list
    /// </summary>
    /// <param name="action">The action being added</param>
    public static void AddAction(Actions.Action action)
    {
        
        instance.actions.Add(action);
    }

    // -------------------------------------- Networking Functions ----------------------------------------------------------
    #region Server Functions
    [ServerRpc]
    public void StartGameDebugServerRPC(int playerCount)
    {

        DebugNetworklLog.Log("Start Game Server");

        // Generate a random seed
        //int randomSeed = (int)Time.time
        int randomSeed = Guid.NewGuid().GetHashCode();



        // Tell the rest of the clients to start their games
        StartGameDebugClientRPC(playerCount,randomSeed);
    }

    #endregion
    #region Client Functions
    [ClientRpc]
    public void StartGameDebugClientRPC(int playerCount, int randomSeed)
    {
        DebugNetworklLog.Log("Starting Game Client");

        // Set the random to match across all the instances of the game
        RNG.SeedRandom(randomSeed);

        PrototypeQuickSetupScript.instance.NetInit(playerCount);
    }


    [ClientRpc]
    public void OnTileClickClientRPC(int nodeID, int mapTileID)
    {
        OnTileClick(nodeID, mapTileID);
    }


    [ClientRpc]
    public void OnPerformCardIn_ClientRPC(TerritoryCard[] cards, int playerID)
    {

    }

    [ClientRpc]
    public void EndTurn_ClientRPC()
    {
        EndTurn();
    }

    [ClientRpc]
    public void ConfirmBattle_ClientRPC(int value)
    {
        // Complete the battle for all players
        CompleteBattle(value);
    }

    [ClientRpc]
    public void ConfirmFortify_ClientRPC(int value)
    {
        // Complete the fortify for all players
        CompleteFortify(value);
    }

    [ClientRpc]
    public void SyncBoardFull_ClientRPC(int[] tileOwners, int[] tileTroops, int playerCount, int[] numOfTroops, int rngSeed)
    {
        DebugNetworklLog.Log("Sync Client Start");
        // Don't worry about syncing as the host
        if (IsHost) { return; }

        // Iterate through all the tiles and adjust them accordingly. 
        for(int i=0; i < tileOwners.Length; ++i)
        {
            // Get the target tile
            var target = gameBoard[i];
            int playerID = tileOwners[i];
            int tileID = i;
            int numOfUnits = tileTroops[i];

            // Set the new owner
            if (target.Owner != players[playerID])
            {
                // Remove the Units (if any)
                if (target.UnitCount > 0)
                {
                    target.KillUnits(target.UnitCount);
                }
                // Change the owner
                gameBoard[tileID].ChangeOwner(players[playerID]);
            }
            // Change the units if they don't match
            if (target.UnitCount != numOfUnits)
            {
                if (target.UnitCount > numOfUnits)
                {
                    target.KillUnits(target.UnitCount - numOfUnits);
                }
                if (target.UnitCount < numOfUnits)
                {
                    target.Fortify(numOfUnits - target.UnitCount);
                }
            }
        }

        // After we set up the count of units, correct player displays
        for(int i=0; i < playerCount; ++i)
        {
            int playerID = i;
            int draftTroops = numOfTroops[i];
            players[playerID].draftTroop = draftTroops;

        }

        // Sync the rng of the two
        RNG.SeedRandom(rngSeed);
    }


    [ClientRpc]
    public void SyncBoardTile_ClientRPC(int tileID, int playerID, int numOfUnits)
    {
        DebugNetworklLog.Log("Sync Board Begin");

        // Don't sync board if host
        if (IsHost) { return; }
        var target = gameBoard[tileID];

        // Set the new owner
        if (target.Owner != players[playerID])
        {
            // Remove the Units (if any)
            if(target.UnitCount > 0)
            {
                target.KillUnits(target.UnitCount);
            }
            // Change the owner
            gameBoard[tileID].ChangeOwner(players[playerID]);
        }
        // Change the units if they don't match
        if(target.UnitCount != numOfUnits)
        {
            if(target.UnitCount > numOfUnits)
            {
                target.KillUnits(target.UnitCount - numOfUnits);
            }
            if(target.UnitCount < numOfUnits)
            {
                target.Fortify(numOfUnits - target.UnitCount);
            }
        }
    }

    [ClientRpc]
    public void SyncPlayer_ClientRPC(int playerID, int draftTroops)
    {
        // Ignore this call if you are the host
        if (IsHost) { return; }
        players[playerID].draftTroop = draftTroops;
    }

    #endregion
    #region Host Functions

    /// <summary>
    /// Reports wheter or not the current player is an AI functioning on this machine
    /// </summary>
    /// <returns>If the current machine is the host and current player is an AI, returns true. Otherwise, returns false. </returns>
    public bool IsCurrentAI()
    {
        return IsHost && !GetPlayer().isHuman;
    }

    public void SyncBoards()
    {
        // Don't call this function if not the host
        if (!IsHost) { return; }

        // Set up the variables

        // The ids of the owners of each tile within the game
        List<int> tileOwners = new List<int>();
        // The number of troops on a given tile
        List<int> tileTroops = new List<int>();

        // Total number of players
        int playerCount;

        List<int> numberOfPlayerUnits = new List<int>();

        // The seed value of the rng at the given time.
        int rngSeed ;


        // Loop through each tile
        for (int i = 0; i < gameBoard.Count; ++i)
        {
            // Call the sync function

            // Tile ID, Tile Owner, Number of Units
            int ownerID = gameBoard[i].Owner != null ? gameBoard[i].Owner.playerID : -1;
            int numberOfUnits = gameBoard[i].UnitCount;

            //DebugNetworklLog.Log("Launching sync");

            //GMNet.Instance.SyncBoards_ServerRPC(i, ownerID, numberOfUnits); ;
            tileOwners.Add(ownerID);
            tileTroops.Add(numberOfUnits);
            //SyncBoardTile_ClientRPC(i, ownerID, numberOfUnits);
        }

        // Set the player count
        playerCount = players.Count;
        //Re-Sync the boards
        for (int i = 0; i < players.Count; ++i)
        {
            //SyncPlayer_ClientRPC(i, players[i].draftTroop);
            //GMNet.Instance.SyncPlayer_ServerRPC(i, players[i].draftTroop);
            numberOfPlayerUnits.Add(players[i].draftTroop);
        }

        // Sync the rng
        rngSeed = RNG.Seed;
        //GMNet.Instance.SyncRNG_ServerRPC(RNG.Seed);

        // Initiate the sync
        GMNet.Instance.SyncBoardsFull_ServerRPC(tileOwners.ToArray(), tileTroops.ToArray(), playerCount, numberOfPlayerUnits.ToArray(), rngSeed);
    }

    #endregion
    // ------------------------------------ Starting Functions ------------------------------------------------------------

    #region Starting Functions
    private void Awake()
    {
        instance = this;
        playerControllers = new List<ClientPlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        hasGameStarted = false;
        actions = new ActionList();
        gameBoard = BoardManager.instance.GetBoard();
    }

    public static void AddPlayerController(ClientPlayerController cpc)
    {
        DebugNetworklLog.Log("Added Controller");
        instance.playerControllers.Add(cpc);
        instance.isNetworked = true;
    }

    public static GameMaster GetInstance()
    {
        return instance;
    }

    public static bool OverrideReinforcement()
    {
        return instance.gameMode.OverrideReinforcement();
    }

    public static void LoadSettings(GameSettings settings)
    {
        instance.settings = settings;
    }

    #endregion

    #region Start Game Functions
    /// <summary>
    /// Starts a game with the current settings and the given number of players
    /// </summary>
    /// <param name="numberOfPlayers">The number of players.</param>
    public void StartGame(int numberOfPlayers)
    {
        StartGame(numberOfPlayers, null);
    }
    public void StartGame(int numberOfPlayers, GameSettings gameSettings)
    {
        StartGame(numberOfPlayers, new AI.AIPlayerData(0), settings, null);
    }

    public void StartGame(int numberOfPlayers,AI.AIPlayerData computerPlayers,GameSettings gameSettings, GameMode mode)
    {
        // Seed the RNG
        RNG.SeedRandom(Guid.NewGuid().GetHashCode());

        if (gameBoard == null)
        {
            gameBoard = BoardManager.instance.GetBoard();
        }
        // Set the game's mode if not available
        if(mode != null)
        {
            gameMode = mode;
        }
        // Set the game settings
        if(gameSettings != null)
        {
            settings = gameSettings;
            DebugNetworklLog.Log("Setting new settings");
            Debug.Log("Settings set");
        }
        // Set the game as started
        hasGameStarted = true;
        PlayerAmount = numberOfPlayers;

        //state = GameState.Claim;
        PlayerAmount += computerPlayers.PlayerCount;

        if (PlayerAmount <= 1)
            PlayerAmount = 2;


        for (int i = 0; i < numberOfPlayers; i++)
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
        // Load in the player count
        for(int i = 0; i < computerPlayers.PlayerCount; ++i)
        {
            Player p = new Player();
            p.playerID = players.Count;
            float t = (float)players.Count / PlayerAmount;
            Color color = Color.HSVToRGB(Mathf.Lerp(0.0f, 1.0f, t), 1.0f, 1.0f);
            color.a = 1.0f;
            p.playerColor = color;
            players.Add(p);

            PlayerPanelUI.SpawnPlayerPanel(p);
            p.isHuman = false;

            // Mark as bot
            AIPlayer bot = new AIPlayer(p);
            p.aiBrain = bot;

            // Start the bot
            p.aiBrain.Initialize(p, this, PersonasDB.RandomPersonality());
            //p.aiBrain.Initialize(p, this, PersonasDB.GetPersonality(1));
        }

        // Connect to player controllers to players
        for(int i=0; i < playerControllers.Count; ++i)
        {
            playerControllers[i].SetPlayerIdentity(players[i]);
            isNetworked = true;
        }

        // Increment the readyCount
       // readyCount.Value = readyCount.Value + 1 ;
        
        // If all players ready, start
        //if(readyCount.Value == playerControllers.Count)
        if(IsNetworked)
        {
            GMNet.Instance.ReadyComplete_ServerRPC();
        }
    }

    [ClientRpc]
    public void ReadyComplete_ClientRPC()
    {
        DebugNetworklLog.Log($"Readying Game");
        ChangeState(GameState.Claim);

        turnTacker = -1;
        IncrementTurnTracker();

        // Only invoke action if someone is listening
        onTurnBegin?.Invoke(turnTacker);
    }
    #endregion

    #region Update

    // Update is called once per frame
    void Update()
    {
        
        if(!hasGameStarted)
        {
            // Choose to initialize local or online game
            if (BoardManager.instance.GetBoard() == null)
            {
                if (IsNetworked && NetworkPlayerDataCarrier.Instance != null)
                {
                    // Set the map of the network
                    GenerateMap.instance.SetMap(NetworkPlayerDataCarrier.Map);
                    // Generate the board
                    GenerateMap.instance.GenBoard();
                }
                else if (DataCarrier.IsValid)
                {
                    // If map is set to null, wait until it is ready
                    if(DataCarrier.GetMap() == null)
                    {
                        return;
                    }
                    GenerateMap.instance.SetMap(DataCarrier.GetMap());
                    GenerateMap.instance.GenBoard();
                }
                else
                {
                    GenerateMap.instance.GenBoard();
                }
                
                return;
            }
            if (GameType.isNetworked)
            {
                NetworkPlayerDataCarrier.InitializeGame(this);
            }
            else
            {
                DataCarrier.InitializeGame(this);
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

        // Networked, make sure other players can't end their turns
        if (IsNetworked)
        {
            if (ClientPlayerController.IsCurrentPlayer(this))
            {
                endTurnButton.interactable = true;
            }
            else
            {
                endTurnButton.interactable = false;
            }
        }


        if (gameMode)
        {
            // Check if the game is over
            if (GetState()!= GameState.Claim && gameMode.CheckIfOver(this, gameBoard) && Winner == null)
            {
                PlayWinSequence(gameMode.WinningPlayer(this, gameBoard));
            }
        }else
        {
            Debug.LogWarning("The Game Mode is missing. If you want to win, you are going to need that");
        }

        // Handle updating AI
        if (GetPlayer().isHuman == false)
        {
            float dt = Time.deltaTime;
            if (!IsNetworked || (IsNetworked && IsCurrentAI()))
            {
                GetPlayer().aiBrain.Update(dt);
            }
        }

    }

    void VisibleUpdateBoard()
    {
        if (settings.FogOfWar)
        {
            Debug.Log("Current Player: " + CurrentPlayer);
            // Update the fog of war
            for (int i = 0; i < gameBoard.Count; ++i)
            {
                gameBoard[i].VisibleUpdate();
            }
        }
    }

    #endregion
    //------------------------------------- Defender and Challenger functions --------------------------------------

    #region Defenders and Challengers

    public void SetDefender(MapTile mt)
    {
        Defender = mt;
        // Do whatever actions you want to 

        
    }

    public void SetChallenger(MapTile mt)
    {
        Challenger = mt;
        // Show the challenger as selected
        Challenger.NodeRef.Select();
        //actions.Add(new Actions.Scale(Vector3.one * 1.4f, Vector3.one, Challenger.gameObject, 0.2f,0.0f, ActionType.NoGroup, false, EaseType.Linear ));
     
        // If I'm in attack mode, highlight the adjacent tiles
        if(state == GameState.Attack)
        {
           // Iterate through all tiles and unhighlight and highlight accordingly
           for(int i =0; i < gameBoard.Count; ++i)
            {
                // Skip if the matching piece
                if(gameBoard[i]== Challenger.NodeRef) { continue; }
                // Add case for blocked/hidden later

                // If adjacent and not sharing the onwer, mark as selectable
                if(Challenger.NodeRef.Neighbors.Contains( gameBoard[i]) && gameBoard[i].Owner != Challenger.Player)
                {
                    gameBoard[i].Selectable(true);
                }    
                else // Otherwise mark as neutral
                {
                    gameBoard[i].Deselect();
                }

            }
        }
        else // Otherwise highlight fortifiable tiles
        {
            // Get the id of the node
            var connectedTiles = gameBoard.GetConnectedTiles(Challenger.NodeRef.ID);


            for(int i =0; i < gameBoard.Count; ++i)
            {
                // Skip if matching
                if(gameBoard[i] == Challenger.NodeRef) { continue; }

                // If node is connected, highlight
                if(connectedTiles.Contains(gameBoard[i]))
                {
                    gameBoard[i].Selectable(true);
                }
                // Otherwise, deselect
                else
                {
                    gameBoard[i].Deselect();
                }

            }
        }

        
        //foreach (MapTile mapTile in FindObjectsOfType<MapTile>())
        //{
        //    if (mt.NodeRef.Neighbors.Contains(mapTile.NodeRef))
        //    {
        //        actions.Add(new Actions.Scale(Vector3.one * 1.1f, Vector3.one, mapTile.gameObject, 0.2f,0.0f, ActionType.NoGroup, false, EaseType.Linear ));
        //    }
        //}
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
            // Apply the de-highlighting effects

            Challenger.NodeRef.Deselect();

            for (int i = 0; i < gameBoard.Count; ++i)
            {
                //  No matter what state, hide the no turn player units
                if(gameBoard[i].Owner != GetPlayer())
                {
                    gameBoard[i].Deselect();
                }
                else
                if(state != GameState.Draft) // If not the start of a new turn
                {
                    // Only highlight those which can attack/fotify
                    if (gameBoard[i].UnitCount > 1)
                    {
                        if (state == GameState.Attack)
                        {
                            bool canAttack = CombatSystem.CanAttack(gameBoard[i]);
                            if (canAttack)
                            {
                                gameBoard[i].Selectable(false);
                            }
                            else
                            {
                                gameBoard[i].Deselect();
                            }
                        }
                        else
                        {
                            gameBoard[i].Selectable(false);
                        }
                    }
                    else
                    {
                        gameBoard[i].Deselect();
                    }
                }
                else // Deselect all when starting new turn
                {
                    gameBoard[i].Deselect();
                }
                //else if(state == GameState.Fortify)
                //{
                //    // Only highlight those which can fortify others
                //    if(gameBoard[i].UnitCount )
                //}
            }

            //    actions.Add(new Actions.Scale(Vector3.one, Vector3.one * 1.4f, Challenger.gameObject, 0.2f,0.0f, ActionType.NoGroup, false, EaseType.Linear ));
            //foreach (MapTile mapTile in FindObjectsOfType<MapTile>())
            //{
            //    if (Challenger.NodeRef.Neighbors.Contains(mapTile.NodeRef))
            //    {
            //        actions.Add(new Actions.Scale(Vector3.one, Vector3.one * 1.1f, mapTile.gameObject, 0.2f,0.0f, ActionType.NoGroup, false, EaseType.Linear ));
            //    }
            //}



            // Reset challenger
            Challenger = null;

            MapDrawSystem.CancelArrow();
        }
    }

    public void ReleaseDefender()
    {
        if(Defender!=null)
        {
            Defender = null;

           //Defender.NodeRef.Deselect();

            // Re-highlight all that is neseccary
            // Iterate through all tiles and unhighlight and highlight accordingly
            for (int i = 0; i < gameBoard.Count; ++i)
            {
                // If the challenger is empty
                if(Challenger == null)
                {
                    if(state == GameState.Attack)
                    {
                        // 
                    }
                    else // Deselect all at the end of the turn
                    {
                        gameBoard[i].Deselect();
                    }

                    // Skip all things below
                    continue;
                }

                // Skip if the matching piece
                if (gameBoard[i] == Challenger.NodeRef) { continue; }
                // Add case for blocked/hidden later

                // If adjacent and not sharing the onwer, mark as selectable
                if (Challenger.NodeRef.Neighbors.Contains(gameBoard[i]) && gameBoard[i].Owner != Challenger.Player)
                {
                    gameBoard[i].Selectable(true);
                }
                else // Otherwise mark as neutral
                {
                    gameBoard[i].Deselect();
                }

            }
        }
    }

    public bool HasChallengerCheck()
    {
        return Challenger != null;
    }

    public bool HasDefenderCheck()
    {
        return Defender != null;
    }

    #endregion

    // ------------------------------------------------- State Changing Functions ----------------------------------------------
    #region State Changing
    public void EndTurn()
    {
        switch (state) {
            case GameState.Claim:
                if (AllTerroritesClaim())
                {
                    ChangeState(GameState.Reinforce);
                    // Update the visual board
                    VisibleUpdateBoard();
                }
                IncrementTurnTracker();

                VisibleUpdateBoard();

                // Let players know if someone owns a whole continent
                UpdateContinentsOwned();
                break;
            case GameState.Reinforce:

                IncrementTurnTracker();
                if (ReinforcingDone())
                    ChangeState(GameState.Draft);
                //onTurnBegin(turnTacker);
                VisibleUpdateBoard();

                break;
            case GameState.Draft: ChangeState(GameState.Attack); break;
            case GameState.Attack: ChangeState(GameState.Fortify); break; 
            case GameState.Fortify: 
                ChangeState(GameState.End);
                if (GetPlayer().canGetCard)
                {
                    CardGainUI.GainCards(CardGainUI.RandomCard(), GetPlayer(),gameBoard);
                }
                else { EndTurn(); }
                break;
            case GameState.End:
                {
                    IncrementTurnTracker(); ChangeState(GameState.Draft);

                    VisibleUpdateBoard();
                    
                    break;
                }
        }

        onTurnBegin?.Invoke(turnTacker);
    }

    public void ForceTurnEnd()
    {
        // Network, otherwise end turn via server
        if(IsNetworked)
        {
            GMNet.Instance.EndTurn_ServerRPC();
        }
        else // End Turn normally if networked
        {
            Debug.Log("Ending turn of " + GetPlayer());
            EndTurn();
        }
    }

    public void UpdateContinentsOwned()
    {
        // Clear all player continents
        for(int i =0; i < players.Count; ++i)
        {
            players[i].continentsOwned.Clear();
        }
        // Loop through and update all of the continents
        for(int i=0; i < gameBoard.ContinentCount; ++i)
        {
            Player owningPlayer = gameBoard.FindContinent(i).GetOwningPlayer();
            // Add the owning player
            if(owningPlayer!=null)
            {
                owningPlayer.continentsOwned.Add(gameBoard.FindContinent(i));
            }
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

    void IncrementTurnTracker()
    {
        if (IsHost)
        {
            // Sync the RNG
            GMNet.Instance.SyncRNG_ServerRPC(RNG.Seed);
        }

        if(turnTacker >=0  && players[turnTacker].isHuman && players[turnTacker].isAlive)
        {
            lastHumanIndex = turnTacker;
        }

        // Increment turn tracker to the next non-dead player
        do
        {
            turnTacker++;
            if (turnTacker >= PlayerAmount)
                turnTacker = 0;
        } while (!players[turnTacker].isAlive);



        // Update the display
        if(gameBoard != null)
        {
            if(state == GameState.Reinforce)
            {
                for(int i=0; i < gameBoard.Count; ++i)
                {
                    if (gameBoard[i].Owner == players[turnTacker]) { gameBoard[i].Selectable(false); }
                    else { gameBoard[i].Deselect(); }
                }
            }
            else if(state == GameState.Claim)
            {
                for(int i =0; i < gameBoard.Count; ++i)
                {
                    if (gameBoard[i].Owner == null) { gameBoard[i].Selectable(false); }
                    else { gameBoard[i].Deselect(); }
                }
            }

        }


        // Let anyone know the turn has changed that needs to 
        //onTurnBegin?.Invoke(turnTacker);
    }

    void ChangeState(GameState desiredState)
    {
        state = desiredState;

        
        if(state == GameState.Claim)
        {
            // Automatically fill the map with this mode
            if(settings.AutoFillTiles)
            {
                if (IsNetworked)
                {
                    // If this is the host/ server, claim and sync
                    if(IsHost)
                    {
                        AutoClaimMap();
                        SyncBoards();
                    }
                }
                else // In a local game
                {

                    AutoClaimMap();
                }

                // Check if we can move on to the next phase
                if(AllTerroritesClaim())
                {
                    //ReinforceSelections();
                    // Enter the next phase
                    ChangeState(GameState.Reinforce);

                    return;
                }


            }
        }
        else if(state == GameState.Reinforce)
        {
            if(settings.AutoReinforce)
            {
                // Reinforce automatically 
                AutoReinforce();
                if(gameMode.ReinforcingComplete(this,gameBoard))
                {
                    DraftSelections();
                    ChangeState(GameState.Draft);
                    if(settings.FogOfWar)
                    {
                        // Update the fog of war
                        for (int i = 0; i < gameBoard.Count; ++i)
                        {
                            gameBoard[i].VisibleUpdate();
                        }
                    }
                    return;
                }
            }
        }
        else if (desiredState == GameState.Draft)
        {
            // Give the base three
            players[turnTacker].draftTroop += GetDraftAmount(players[turnTacker]);
            //players[turnTacker].draftTroop += 3;

            //// Add the additional troops from the continents
            //foreach(MapSystem.Continent con in players[turnTacker].continentsOwned)
            //{
            //    int total = 0;
            //    foreach(BonusBase bonus in con.AllBonuses)
            //    {
            //        // If not a unit bonus, ignore

            //        if (bonus.MyType != BonusBase.BonusType.Unit) { continue; }
            //        // Try to cast to unit bonus
            //        UnitBonus b = (UnitBonus)bonus;

            //        // Otherwise add to total
            //        total += b.Count;
            //    }

            //    players[turnTacker].draftTroop += total;
            //}

            //int tileCount = 0;
            //// Search through all of the player's tiles for additional bonuses
            //for(int i =0; i < gameBoard.Count; ++i)
            //{
            //    // The tile belongs to the given player
            //    if(gameBoard[i].Owner == GetPlayer())
            //    {
            //        // Read the in the bonuses to units
            //        BonusBase bonus = gameBoard[i].GetBonusOfType(BonusBase.BonusType.Unit);
            //        if (bonus != null)
            //        {
            //            // Adds the additional units
            //            players[turnTacker].draftTroop += ((UnitBonus)bonus).Count; 
            //        }
            //        // Count the tile
            //        tileCount++;
            //    }
            //}



            //// Get plus one unit per X tiles
            //players[turnTacker].draftTroop += tileCount/gameMode.TilesPerTroop;

            // Show locations
            if(gameBoard != null)
            {
                for(int i =0; i < gameBoard.Count; ++i )
                {
                    if(gameBoard[i].Owner == players[turnTacker]) { gameBoard[i].Selectable(false); }
                    else { gameBoard[i].Deselect(); }
                }
            }
        }
        else if (state == GameState.Attack )
        {
            for (int i = 0; i < gameBoard.Count; ++i)
            {
                if (gameBoard[i].Owner != players[turnTacker]) { gameBoard[i].Deselect(); }
                else if (gameBoard[i].UnitCount <= 1) { gameBoard[i].Deselect(); }
                else 
                {
                    bool canAttack =CombatSystem.CanAttack(gameBoard[i]);

                    if (canAttack)
                    {
                        gameBoard[i].Selectable(false);
                    }
                    else
                    {
                        gameBoard[i].Deselect();
                    }
                }
            }
        }
        else if ( state == GameState.Fortify)
        {
            for (int i = 0; i < gameBoard.Count; ++i)
            {
                if (gameBoard[i].Owner != players[turnTacker]) { gameBoard[i].Deselect(); }
                else if (gameBoard[i].UnitCount <= 1) { gameBoard[i].Deselect(); }
                else { gameBoard[i].Selectable(false); }
            }
        }


    }

    #endregion

    //---------------------------------------- On Click Functions ----------------------------------------



    public void OnTileClick(int tileIndex)
    {
        if (IsNetworked)
        {
            OnTileClickClientRPC(tileIndex, GenerateMap.GetTile(gameBoard[tileIndex]).ID);  
        }
        else
        {
            // Return the tile index
            OnTileClick(tileIndex, GenerateMap.GetTile(gameBoard[tileIndex]).ID);
        }
    }

    public void OnTileClick(int tileIndex, int mapTileID)
    {
        switch (GetState())
        {
            case GameState.Claim: ClaimTiles(gameBoard[tileIndex]); break;
            case GameState.Reinforce: ReinforceTile(gameBoard[tileIndex]); break;
            case GameState.Draft: DraftTile(gameBoard[tileIndex]); break;
            case GameState.Attack: AttackTile(GenerateMap.GetTile(mapTileID)); break;
            case GameState.Fortify: FortifyTile(GenerateMap.GetTile(mapTileID)); break;
        }
    }

    // --------------------------------------- Claiming Functions -------------------------------------------
    #region Claiming Functions
    Player GetNextDraftablePlayer(int index)
    {
        // Return the player index
        if (players[index].draftTroop > 0) { return players[index]; }

        int newIndex = index + 1;

        while (newIndex != index)
        {
            if (newIndex >= players.Count) { newIndex = 0; }
            // IF this player has troops, deploy those troops
            if (players[newIndex].draftTroop > 0) { return players[newIndex]; }

            newIndex++;
        }


#if UNITY_EDITOR
        // If no one else is available, throw an exception
        throw new Exception("Unable to find a player with troops");
#else
        return players[index];
#endif
    }


    public void AutoClaimMap()
    {
        int openTiles = gameBoard.Count;

        int i = 0;
        while (openTiles > 0 && i < gameBoard.Count)
        {
            // Fix later
            Player p = GetNextDraftablePlayer(RNG.Roll(0, PlayerAmount - 1));
            gameBoard[i].ChangeOwner(p);
            gameBoard[i].AddUnits(new Unit(1));

            // Decrement troop
            p.draftTroop--;

            // Decrement open tiles
            openTiles--;
            // Increment i
            ++i;
        }
    }

    void ClaimSelections()
    {
        for (int i = 0; i < gameBoard.Count; ++i)
        {
            if (gameBoard[i].Owner == null) { gameBoard[i].Selectable(false); }
            else { gameBoard[i].Deselect(); }
        }
    }

    public void ClaimTiles(MapSystem.BoardTile tile)
    {
        if (tile.Owner == null)
        {
            tile.ChangeOwner(GetPlayer());
            tile.Owner.draftTroop--;
            tile.AddUnits(new Unit(1));

            EffectSystem.SpawnText(tile.Position, tile.Owner.playerColor).Text = "+1";

            EndTurn();
        }
    }
    #endregion
    //--------------------------------------------- Reinforcing Functions --------------------------------
    #region Reinforcing
    bool ReinforcingDone()
    {
        foreach (Player player in players)
        {
            if (player.draftTroop > 0)
                return false;
        }

        // If the game mode is not ready to finish reinforcing, then we are not done
        if (!gameMode.ReinforcingComplete(this, gameBoard))
        {
            return false;
        }

        return true;
    }

    public void AutoReinforce()
    {

        

        // Generate lists of tiles for each player
        List<MapSystem.BoardTile>[] tiles = new List<MapSystem.BoardTile>[PlayerAmount];
        for(int i=0; i <gameBoard.Count; ++i)
        {
            // Skip ownerless tiles
            if(gameBoard[i].Owner == null) { continue; }
            // Create the list if it doesn't exist
            if(tiles[gameBoard[i].Owner.playerID] == null) { tiles[gameBoard[i].Owner.playerID] = new List<MapSystem.BoardTile>(); }
            tiles[gameBoard[i].Owner.playerID].Add(gameBoard[i]);
        }

        // Then loop through each of the players and randomly decrement draft troops
        for(int i=0; i < tiles.Length; ++i)
        {
            while(players[i].draftTroop > 0)
            {
                // Add one draft troop to a random tile
                tiles[i][RNG.Roll(0, tiles[i].Count-1)].Fortify(1);
                //players[i].OnTroopCountChange(1);
                players[i].draftTroop--;
            }
        }
        

        
    }    
    
    void ReinforceSelections()
    {
        for (int i = 0; i < gameBoard.Count; ++i)
        {
            if (gameBoard[i].Owner == players[turnTacker]) { gameBoard[i].Selectable(false); }
            else { gameBoard[i].Deselect(); }
        }
    }

    public void ReinforceTile(MapSystem.BoardTile tile)
    {
        if (GetPlayerTurn() == tile.Owner.playerID)
        {
            if (tile.Owner.draftTroop <= 0 || GameMaster.OverrideReinforcement())
            {
                GameMaster.GetInstance().GameModeReinforce(tile);
            }
            else
            {
                tile.Fortify(1);
                EffectSystem.SpawnText(tile.Position, tile.Owner.playerColor).Text = $"+1";

                tile.Owner.draftTroop--;
                //tile.Owner.OnTroopCountChange(1);
                EndTurn();
            }
        }
    }

    #endregion
    //-------------------------------------------- Draft Functions ------------------------------------

    #region Draft Functions
    void DraftSelections()
    {
        //ReinforceSelections();
    }

    public void DraftTile(MapSystem.BoardTile tile)
    {
        if (GetPlayerTurn() == tile.Owner.playerID)
        {
            tile.Fortify(1);
            //tile.Owner.OnTroopCountChange(1);
            EffectSystem.SpawnText(tile.Position, tile.Owner.playerColor).Text = $"+1";

            tile.Owner.draftTroop--;

            if (tile.Owner.draftTroop <= 0)
                EndTurn();
        }
    }

    public int GetDraftAmount(Player player)
    {
        // Give the base three
        int draftTroop = 3;


        // Add the additional troops from the continents
        foreach (MapSystem.Continent con in players[turnTacker].continentsOwned)
        {
            int total = 0;
            foreach (BonusBase bonus in con.AllBonuses)
            {
                // If not a unit bonus, ignore

                if (bonus.MyType != BonusBase.BonusType.Unit) { continue; }
                // Try to cast to unit bonus
                UnitBonus b = (UnitBonus)bonus;

                // Otherwise add to total
                total += b.Count;
            }

            draftTroop += total;
        }

        int tileCount = 0;
        // Search through all of the player's tiles for additional bonuses
        for (int i = 0; i < gameBoard.Count; ++i)
        {
            // The tile belongs to the given player
            if (gameBoard[i].Owner == player)
            {
                // Read the in the bonuses to units
                BonusBase bonus = gameBoard[i].GetBonusOfType(BonusBase.BonusType.Unit);
                if (bonus != null)
                {
                    // Adds the additional units
                    draftTroop += ((UnitBonus)bonus).Count;
                }
                // Count the tile
                tileCount++;
            }
        }
        // Get plus one unit per X tiles
        draftTroop += tileCount / gameMode.TilesPerTroop;
        return draftTroop;

    }

    #endregion

    //-------------------------------------------- Attack Functions -------------------------------------------
    #region Attack Functions

    public void AttackTile(MapTile mapTile)
    {
        if (HasChallengerCheck())
        {
            //Has a Challenger
            if (GetChallenger() == mapTile)
            {
                ReleaseChallenger();
                // Stop drawing the cancel arrow
                MapDrawSystem.CancelArrow();
                ConfirmUI.CancelConfirm();
                isInBattle = false;
            }
            else
            {
                // Battle two challengers
                if (Challenger.Player.playerID != mapTile.Player.playerID)
                {
                    if (Challenger.NodeRef.Neighbors.Contains(mapTile.NodeRef))
                    {
                        // Mark this tile as the challenger
                        SetDefender(mapTile);

                        // Draw the arrow displaying who is attacking on the map
                        MapDrawSystem.SpawnArrow(GetChallenger().NodeRef.Position, mapTile.NodeRef.Position);
                        isInBattle = true;
                        // Only pull up the UI if you are the turn player
                        if ((!IsNetworked && GetPlayer().isHuman) 
                            ||(IsNetworked && (ClientPlayerController.IsCurrentPlayer(this) )))
                        {

                            // Pull up the confirm menu
                            ConfirmUI.BeginConfirm($"Attack {mapTile.NodeRef.Name}?", ConfirmUI.ConfirmType.Battle, GetChallenger().NodeRef);
                        }

                    }
                }
            }

        }
        else
        {
            if (GetPlayerTurn() == mapTile.Player.playerID)
            {

                // Only allow if the unit count is greater than 1
                if (!CombatSystem.CanAttack(mapTile.NodeRef)) { return; }

                //Doesn't Have a Challenger
                SetChallenger(mapTile);
            }
        }
    }
    #endregion

    //------------------------------------------------- Fortify Functions ---------------------------------------
    #region Fortify Functions
    public void FortifyTile(MapTile mapTile)
    {
        if (GetPlayerTurn() == mapTile.Player.playerID)
        {
            if (HasChallengerCheck())
            {
                // Do not let
                if (GetChallenger() == mapTile)
                {
                    // Prevent the deselection of fortification during the attack phase
                    if (GetState() != GameState.Attack)
                    {
                        ReleaseChallenger();
                        MapDrawSystem.CancelArrow();
                        ConfirmUI.CancelConfirm();
                    }
                }
                else
                {
                    MapSystem.Board board = BoardManager.instance.GetBoard();
                    // Check if the defender is connected to the challenger
                    if (!board.GetConnectedTiles(mapTile.NodeRef.ID).Contains(GetChallenger().NodeRef))
                    {
                        // Don't do anything if it does not
                        return;
                    }

                    //NodeRef.TransferUnits(gm.GetChallenger().NodeRef, 1);
                    ////gm.GetChallenger().Units = gm.GetChallenger().NodeRef.UnitCount;
                    ////Units = NodeRef.UnitCount;
                    //gm.ReleaseChallenger();
                    // Mark this tile as the challenger
                    SetDefender(mapTile);

                    // Draw the arrow displaying who is attacking on the map
                    MapDrawSystem.SpawnArrow(GetChallenger().NodeRef.Position, mapTile.NodeRef.Position);

                    // If you are the current player, open up the attack menu
                    if (!IsNetworked || (IsNetworked && (ClientPlayerController.IsCurrentPlayer(this))))
                    {
                        if (GetPlayer().isHuman)
                        {
                            // Pull up the confirm menu
                            ConfirmUI.BeginConfirm("Fortify", ConfirmUI.ConfirmType.Fortify, GetChallenger().NodeRef, mapTile.NodeRef);
                        }
                    }
                }
            }
            else
            {
                // Only allow if the unit count is greater than 1
                if (mapTile.NodeRef.UnitCount <= 1) { return; }

                //Doesn't Have a Challenger
                SetChallenger(mapTile);
            }
        }
    }

    #endregion

    // -------------------------------------- Getter Functions ---------------------------------------------
    #region Getter Functions
    public int GetPlayerTurn()
    {
        return turnTacker;
    }

    public GameState GetState()
    {
        return state;
    }

    public GameMode GetMode()
    {
        return gameMode;
    }

    public Player GetPlayer()
    {
        return players[turnTacker];
    }

    public Player GetLastHumanPlayer()
    {
        return players[lastHumanIndex];
    }

    public Player GetPlayerAt(int index)
    {
        return players[index];
    }

#endregion

    // --------------------------------------- Debug Functions ----------------------------------------------
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(BeginningOfUILine,0.5f);
        Gizmos.DrawSphere(EndOfUILine,0.5f);
        Gizmos.DrawLine(BeginningOfUILine, EndOfUILine);
    }
    // --------------------------------------- Game Mode Functions -----------------------------------------------

    /// <summary>
    /// Asks the game mode to perform its own reinforcement step in-place of the standard one
    /// </summary>
    /// <param name="tile">The tile the reinforce step is targeting.</param>
    public void GameModeReinforce(MapSystem.BoardTile tile)
    {

        // Perform reinforcing within the game mode
        gameMode.PerformReinforcingStep(this, gameBoard, tile);
    }


    //------------------------------------------ Combat Functions -------------------------------------------
    #region Combat Functions
    public void CompleteBattle(int index =-1, int other =1)
    {

        int AtkUnitsLost = 0, DefUnitsLost = 0;
        CombatSystem.BattleTiles(GetChallenger().NodeRef, Defender.NodeRef,(Combat.CombatRollType)(-index), out AtkUnitsLost, out DefUnitsLost,other);

        EffectSystem.SpawnText(Challenger.NodeRef.Position, Challenger.Player.playerColor).Text = $"-{AtkUnitsLost}";
        EffectSystem.SpawnText(Defender.NodeRef.Position, Defender.Player.playerColor).Text = $"-{DefUnitsLost}";

      

        if (Defender.Units <= 0)
        {
            
            // Update the player
            onAfterAttack?.Invoke(turnTacker, Defender.NodeRef.ID);

            // If we have captured a territory, allow us to gain a card
            GetPlayer().canGetCard = true;

            Player priorPlayer = Defender.NodeRef.Owner;
           

            Defender.SetOwner(GetChallenger().Player);

            if(priorPlayer.territoryCount <=0)
            {
                PlayerLossUI.PlayPlayerLoss(priorPlayer, 10.0f);
            }

            int unitsToMove = Mathf.Clamp((-index), 1, 3);

            // If we are networked, don't open network ui
            if (!IsNetworked || (IsNetworked && (ClientPlayerController.IsCurrentPlayer(this) || IsCurrentAI())))
            {
                if (GetPlayer().isHuman)
                {
                    // Pull up the confirm for fortify
                    ConfirmUI.BeginConfirm("Fortify", ConfirmUI.ConfirmType.Fortify,
                        GetChallenger().NodeRef, GetDefender().NodeRef,
                        Mathf.Clamp(unitsToMove, 1, Challenger.Units - 1));
                }

                //Defender.NodeRef.Fortify(3);
            }

        }
        else
        {
            // Update the player
            onAfterAttack?.Invoke(turnTacker, Challenger.NodeRef.ID);

            // Perform the proper cleanup
            ReleaseChallenger();
            ReleaseDefender();
            MapDrawSystem.CancelArrow();
            // Battle is over
            isInBattle = false;
        }
    }

    #endregion

    // ------------------------------------------ Fortify Functions ----------------------------------------------

    #region Fortify Functions
    public void CompleteFortify(int amount)
    {
        // Transfer over the units
        Defender.NodeRef.TransferUnits( Challenger.NodeRef, amount);
        EffectSystem.SpawnText(Challenger.NodeRef.Position, Challenger.Player.playerColor).Text = $"-{amount}";
        EffectSystem.SpawnText(Defender.NodeRef.Position, Defender.Player.playerColor).Text = $"+{amount}";

        ReleaseChallenger();
        ReleaseDefender();
        MapDrawSystem.CancelArrow();

        // Force continue after you fortify
        if(GetState() == GameState.Fortify)
        {
            EndTurn();
        }
        // Update continents
        if(GetState() == GameState.Attack)
        {
            UpdateContinentsOwned();
        }

        // Mark battle as over
        isInBattle = false;
    }

    #endregion

    // ------------------------------------------ Confirm Functions --------------------------------------------
    #region Confirm Functions
    public void Confirm(int value, int otherV =1)
    {
        switch(GetState())
        {
            // Handle combat confirmation
            case GameState.Attack:
                {
                    if (value <= 0)
                    {
                        if(IsNetworked)
                        {
                            GMNet.Instance.ConfirmBattle_ServerRPC(value);
                            return;
                        }
                       
                        // NOTE: Will want to make fortify changes here as well.
                        CompleteBattle(value,otherV);
                    }
                    else
                    {
                        if (IsNetworked)
                        {
                            GMNet.Instance.ConfirmFortify_ServerRPC(value);
                            return;
                        }
                        CompleteFortify(value);
                    }

                    break;
                }
            case GameState.Fortify:
                {
                    if (IsNetworked)
                    {
                        GMNet.Instance.ConfirmFortify_ServerRPC(value);
                        return;
                    }
                    CompleteFortify(value);
                    break;
                }
                
        }
    }

    #endregion

    // ------------------------------------------- Win Functions ----------------------------------------------
    #region Win Loss Functions
    public void ClosePlayerLoss()
    {
        // If that was the winning play, end the game
        if(Winner != null)
        {
            // Perform Win
            PlayWinSequence(Winner);
        }
        else if(lastDeadPlayer.cards.Count > 0)
        {
            // Give the player of this turn those cards
            CardGainUI.GainCards(lastDeadPlayer.cards, GetPlayer(), gameBoard);
        }
    }

    public void PlayWinSequence(Player winner)
    {
        // Set the winner
        winningPlayer = winner;
        // Don't  play the animation if the loss UI is active
        if (PlayerLossUI.IsActive) { return; }

        // Display Win Text
        WinLossUI.Open("Victory!", true, winner);
        // Shut everything down
    }

    public void PlayWinSequence()
    {
        // Display Win Text
        WinLossUI.Open("Victory!", true, Winner);
        // Shut everything down
    }
    #endregion
}

public enum GameState{
    Claim,
    Reinforce,
    Draft,
    Attack,
    Fortify,
    End
}

public class Player
{
    public Player()
    {
        troopCount = 0;
        isAlive = true;
        draftTroop = 20;
        continentsOwned = new List<MapSystem.Continent>();
        cards = new List<TerritoryCard>();

        isHuman = true;

        GameMaster.GetInstance().onAfterAttack += OnAttack;
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
    public List<MapSystem.Continent> continentsOwned;

    public List<MapSystem.BoardTile> tiles = new List<MapSystem.BoardTile>();

    public Sprite playerIcon;

    public AIPlayer aiBrain;

    public bool canGetCard;
    public int territoryCount;
    // Insert spot for player UI

    // Actions

    public System.Action<int> onTroopCountChange;
    public System.Action<int, int> onTerritoryCountChange;
   

    public void ChangeTileOwner(MapSystem.BoardTile changedTile)
    {
        if (tiles.Contains(changedTile)) 
        { 
            tiles.Remove(changedTile); 

            // Update the AI to know what continents it is on
            if(!isHuman)
            {
                MapSystem.Board board = BoardManager.instance.GetBoard();
                MapSystem.Continent con = board.FindContinent(changedTile);

                // Check if we have any other tiles in the owner
                for(int i=0; i < tiles.Count; ++i)
                {
                    // Check if the tile is in the continent
                    if(con.Contains(tiles[i]))
                    {
                        // Then we don't need to do anything
                        return;   
                    }
                }

                // If not, note that we don't in world state
                aiBrain.UpdateWorldState(AI.StateKeys.ExistsIn, con, false);
            }
        }
        else 
        { 
            tiles.Add(changedTile); 
            // Update the AI to know what continent it is on
            if(!isHuman)
            {
                MapSystem.Board board = BoardManager.instance.GetBoard();
                MapSystem.Continent con = board.FindContinent(changedTile);

                // Check if we have any other tiles in the con
                for(int i=0; i <tiles.Count; ++i)
                {
                    // Ignore the new tile
                    if(tiles[i] == changedTile) { continue; }
                    // Check if the tile is in the continent
                    if(con.Contains(tiles[i]))
                    {
                        return;
                    }
                }

                // If not, add to the world state
                aiBrain.UpdateWorldState(AI.StateKeys.ExistsIn, con, true);

            }
        }
    }

    public void OnContinentOwn(MapSystem.Continent con)
    {
        // If I already owned you, don't do anything
        if (continentsOwned.Contains(con)) { return; }

        // Add to owned continents
       // continentsOwned.Add(con);

        // If not a player, update the ai
        if(!isHuman)
        {
            aiBrain.UpdateWorldState(AI.StateKeys.Owns, con,true);
        }

    }

    public void OnContinentDisown(MapSystem.Continent con)
    {
        // If don't already own you, don't do anything
        if (!continentsOwned.Contains(con)) { return; }

        // Add to owned continents
      //s  continentsOwned.Remove(con);

        // If not a player, update the ai
        if (!isHuman)
        {
            aiBrain.UpdateWorldState(AI.StateKeys.Owns, con, false);
        }

        

    }

    public void OnAttack(int index, int attackNodeIndex)
    {
        // If the player index doesn't match, return
        if(index != playerID) { return; }

        if(!isHuman)
        {
            // Mention the last attack
            //var board = BoardManager.instance.GetBoard();

            aiBrain.Blackboard.UpdateValue("LastAttackTileID", attackNodeIndex);

            
        }
    }

    public void OnTroopCountChange(int valueMod)
    {
        troopCount += valueMod;

        onTroopCountChange?.Invoke(troopCount);
    }

    public void OnTileCountChange(int value)
    {
        territoryCount += value;
        // Get a list of bonuses on the territories
        //List<BonusBase> bonuses = new List<BonusBase>();
        int bonusTroops = 0;

        // On Territory Count change
        onTerritoryCountChange?.Invoke(territoryCount,bonusTroops);
    }




    public override string ToString()
    {
        return "Player " + playerID;
    }

    public bool isHuman;
}