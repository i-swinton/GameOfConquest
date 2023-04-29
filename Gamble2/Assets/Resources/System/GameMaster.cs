using System;
using System.Collections;
using System.Collections.Generic;
using Actions;
using TMPro;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public Player Winner { get { return winningPlayer; } }

    Player lastDeadPlayer;

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

    [Header("Game Settings")]
    [SerializeField]
    GameMode gameMode;
    MapSystem.Board gameBoard;

    [SerializeField]
    GameSettings settings;

    bool hasGameStarted;

    // Singleton
    static GameMaster instance;

    /// <summary>
    /// Add an action to the central action list
    /// </summary>
    /// <param name="action">The action being added</param>
    public static void AddAction(Actions.Action action)
    {
        
        instance.actions.Add(action);
    }

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

    public static bool OverrideReinforcement()
    {
        return instance.gameMode.OverrideReinforcement();
    }

    public static void LoadSettings(GameSettings settings)
    {
        instance.settings = settings;
    }

    public void StartGame(int numberOfPlayers)
    {
        if(gameBoard == null)
        {
            gameBoard = BoardManager.instance.GetBoard();
        }
        // Set the game as started
        hasGameStarted = true;
        PlayerAmount = numberOfPlayers;

        //state = GameState.Claim;

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

        ChangeState(GameState.Claim);

        turnTacker = -1;
        IncrementTurnTracker();
        //ChangeState(GameState.Claim);

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
            if (GetState()!= GameState.Claim && gameMode.CheckIfOver(this, gameBoard) && Winner == null)
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

    public void EndTurn()
    {
        switch (state) {
            case GameState.Claim:
                if(AllTerroritesClaim())
                    ChangeState(GameState.Reinforce);
                IncrementTurnTracker();
                // Let players know if someone owns a whole continent
                UpdateContinentsOwned();
                break;
            case GameState.Reinforce:
                IncrementTurnTracker();
                if(ReinforcingDone())
                    ChangeState(GameState.Draft);
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
            case GameState.End: IncrementTurnTracker(); ChangeState(GameState.Draft); break;
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
    }

    void ChangeState(GameState desiredState)
    {
        state = desiredState;

        
        if(state == GameState.Claim)
        {
            // Automatically fill the map with this mode
            if(settings.AutoFillTiles)
            {
                AutoClaimMap();
                
                // Check if we can move on to the next phase
                if(AllTerroritesClaim())
                {
                    ReinforceSelections();
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
                    return;
                }
            }
        }
        else if (desiredState == GameState.Draft)
        {
            // Give the base three
            players[turnTacker].draftTroop += 3;

            // Add the additional troops from the continents
            foreach(MapSystem.Continent con in players[turnTacker].continentsOwned)
            {
                int total = 0;
                foreach(BonusBase bonus in con.AllBonuses)
                {
                    // If not a unit bonus, ignore

                    if (bonus.MyType != BonusBase.BonusType.Unit) { continue; }
                    // Try to cast to unit bonus
                    UnitBonus b = (UnitBonus)bonus;

                    // Otherwise add to total
                    total += b.Count;
                }

                players[turnTacker].draftTroop += total;
            }

            int tileCount = 0;
            // Search through all of the player's tiles for additional bonuses
            for(int i =0; i < gameBoard.Count; ++i)
            {
                // The tile belongs to the given player
                if(gameBoard[i].Owner == GetPlayer())
                {
                    // Read the in the bonuses to units
                    BonusBase bonus = gameBoard[i].GetBonusOfType(BonusBase.BonusType.Unit);
                    if (bonus != null)
                    {
                        // Adds the additional units
                        players[turnTacker].draftTroop += ((UnitBonus)bonus).Count; 
                    }
                    // Count the tile
                    tileCount++;
                }
            }



            // Get plus one unit per X tiles
            players[turnTacker].draftTroop += tileCount/gameMode.TilesPerTroop;

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

    // --------------------------------------- Claiming Functions -------------------------------------------
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



    #endregion
    //-------------------------------------------- Draft Functions ------------------------------------
   void DraftSelections()
    {
        ReinforceSelections();
    }



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

    public Player GetPlayer()
    {
        return players[turnTacker];
    }

    public Player GetPlayerAt(int index)
    {
        return players[index];
    }

#endregion
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
    public void CompleteBattle(int index =-1)
    {

        int AtkUnitsLost = 0, DefUnitsLost = 0;
        CombatSystem.BattleTiles(GetChallenger().NodeRef, Defender.NodeRef,(Combat.CombatRollType)(-index), out AtkUnitsLost, out DefUnitsLost);

        EffectSystem.SpawnText(Challenger.NodeRef.Position, Challenger.Player.playerColor).Text = $"-{AtkUnitsLost}";
        EffectSystem.SpawnText(Defender.NodeRef.Position, Defender.Player.playerColor).Text = $"-{DefUnitsLost}";


        if (Defender.Units <= 0)
        {
            // If we have captured a territory, allow us to gain a card
            GetPlayer().canGetCard = true;

            Player priorPlayer = Defender.NodeRef.Owner;
           

            Defender.SetOwner(GetChallenger().Player);

            if(priorPlayer.territoryCount <=0)
            {
                PlayerLossUI.PlayPlayerLoss(priorPlayer, 10.0f);
            }

            int unitsToMove = Mathf.Clamp((-index), 1, 3);

            // Pull up the confirm for fortify
            ConfirmUI.BeginConfirm("Fortify", ConfirmUI.ConfirmType.Fortify,
                GetChallenger().NodeRef, GetDefender().NodeRef,
                Mathf.Clamp(unitsToMove , 1, Challenger.Units-1));
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
        troopCount = 5;
        isAlive = true;
        draftTroop = 20;
        continentsOwned = new List<MapSystem.Continent>();
        cards = new List<TerritoryCard>();
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

    public Sprite playerIcon;

    public bool canGetCard;
    public int territoryCount;
    // Insert spot for player UI
}