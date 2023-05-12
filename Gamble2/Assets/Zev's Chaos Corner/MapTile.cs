using System;
using System.Collections;
using System.Collections.Generic;
using Combat;
using MapSystem;
using UnityEngine;
using TMPro;
public class MapTile : MonoBehaviour
{
    public Player Player
    {
        get
        {
            if (NodeRef == null) { return null; }
            return NodeRef.Owner;
        }
    }
    private string Name;

    public int Units
    {
        get
        {
            if (NodeRef == null) { return 0; }
            return NodeRef.UnitCount;
        }
    }


    public MapSystem.BoardTile NodeRef;
    PolygonCollider2D Colid;
    public Vector3 Center = new Vector3(1,1,1);

    public Transform UIDisplays;
    public TextMeshPro NameDisplay;
    public TextMeshPro UnitsDisplay;

    private SpriteRenderer _spriteRenderer;
    private GameMaster gm;

    public int ID
    {
        get
        {
            return id;
        }
    }

    int id;

    public void SetIndex(int ind)
    {
        id = ind;
    }

    // Start is called before the first frame update
    public void GenTile(MapSystem.BoardTile nodeRef, string name)
    {
        NodeRef = nodeRef;
        Name = name;

        // Initialize the actions to the noderef
        MapTileRender mtr = GetComponent<MapTileRender>();
        NodeRef.onSelect += mtr.Select;
        NodeRef.onDeselect += mtr.Deselect;
        NodeRef.onSelectable += mtr.CanSelect;
        // Insert Unselectable here



        Colid = GetComponent<PolygonCollider2D>();

        Center = Colid.bounds.center;

        UIDisplays.position = Center + Vector3.back;
        NameDisplay.text = Name;

        gm = FindObjectOfType<GameMaster>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Player != null)
        {
            _spriteRenderer.color = Player.playerColor;
            UnitsDisplay.text = Units.ToString();
        }
    }

    public void OnClick()
    {
        // If the current player in a networked game is not the turn player, don't do anything
        if (GameMaster.GetInstance().IsNetworked)
        {

            DebugNetworklLog.Log($"Turn is {GameMaster.GetInstance().GetPlayer().playerID} player {ClientPlayerController.LocalPlayer}.");
            
            if (!ClientPlayerController.IsCurrentPlayer(gm))
            {
              //  DebugNetworklLog.Log("Is not current player");
                return;
            }

            //DebugNetworklLog.Log("Continue");

            // Send the RPC to the GMNet belonging to this player
            GMNet.Instance.OnMapTileClickServerRPC(NodeRef.ID, ID); 
        }
        else
        {
            // Only allow interactions if a human player is playing
            {
                if (gm.GetPlayer().isHuman)
                {
                    gm.OnTileClick(NodeRef.ID, ID);
                }
            }
        }
        //switch (gm.GetState())
        //{
        //    case GameState.Claim: MouseDown_Claim(); break;
        //    case GameState.Reinforce: MouseDown_Reinforce(); break;
        //    case GameState.Draft: MouseDown_Draft(); break;
        //    case GameState.Attack: MouseDown_Attack(); break; 
        //    case GameState.Fortify: MouseDown_Fortify();  break;
        //}
    }

    void MouseDown_Claim()
    {
        // If we are networked, only allow the player to click on their turn
        if(GameMaster.GetInstance().IsNetworked)
        {
            // Check if the player does not match the current player
            if(!ClientPlayerController.IsCurrentPlayer(gm))
            {
                return;
            }
        }
        else if (Player == null)
        {
            gm.ClaimTiles(NodeRef);
        }
    }
    
    void MouseDown_Reinforce()
    {
        //if (gm.GetPlayerTurn() == Player.playerID)
        //{
        //    if (Player.draftTroop <= 0 || GameMaster.OverrideReinforcement())
        //    {
        //        GameMaster.GetInstance().GameModeReinforce(NodeRef);
        //    }
        //    else
        //    {
        //        NodeRef.Fortify(1);
        //        EffectSystem.SpawnText(NodeRef.Position, Player.playerColor).Text = $"+1";

        //        Player.draftTroop--;
        //        gm.EndTurn();
        //    }
        //}
        // If we are networked, only allow the player to click on their turn
        if (GameMaster.GetInstance().IsNetworked)
        {
            // Check if the player does not match the current player
            if (!ClientPlayerController.IsCurrentPlayer(gm))
            {
                return;
            }
        }
         if (Player == null)
        {
            gm.ReinforceTile(NodeRef);
        }
    }
    
    void MouseDown_Draft()
    {
        //if (gm.GetPlayerTurn() == Player.playerID)
        //{
        //    NodeRef.Fortify(1);
        //    EffectSystem.SpawnText(NodeRef.Position, Player.playerColor).Text = $"+1";

        //    Player.draftTroop--;

        //    if (Player.draftTroop <= 0)
        //        gm.EndTurn();
        //}
        // If we are networked, only allow the player to click on their turn
        if (GameMaster.GetInstance().IsNetworked)
        {
            // Check if the player does not match the current player
            if (!ClientPlayerController.IsCurrentPlayer(gm))
            {
                return;
            }
        }
        {
            gm.DraftTile(NodeRef);
        }
    }
    
    void MouseDown_Attack()
    {
        if (GameMaster.GetInstance().IsNetworked)
        {
            // Check if the player does not match the current player
            if (!ClientPlayerController.IsCurrentPlayer(gm))
            {
                return;
            }
        }
         if (Player == null)
        {
            gm.AttackTile(this);
        }
        //if (gm.HasChallengerCheck())
        //{
        //    //Has a Challenger
        //    if (gm.GetChallenger() == this)
        //    {
        //        gm.ReleaseChallenger();
        //        // Stop drawing the cancel arrow
        //        MapDrawSystem.CancelArrow();
        //        ConfirmUI.CancelConfirm();
        //    }
        //    else
        //    {
        //        // Battle two challengers
        //        if (gm.GetChallenger().Player.playerID != Player.playerID)
        //        {
        //            if (gm.GetChallenger().NodeRef.Neighbors.Contains(NodeRef))
        //            {
        //                // Mark this tile as the challenger
        //                gm.SetDefender(this);

        //                // Draw the arrow displaying who is attacking on the map
        //                MapDrawSystem.SpawnArrow(gm.GetChallenger().NodeRef.Position, NodeRef.Position);

        //                // Pull up the confirm menu
        //                ConfirmUI.BeginConfirm($"Attack {NodeRef.Name}?", ConfirmUI.ConfirmType.Battle,gm.GetChallenger().NodeRef);

        //            }
        //        }
        //    }

        //}
        //else
        //{
        //    if (gm.GetPlayerTurn() == Player.playerID)
        //    {

        //        // Only allow if the unit count is greater than 1
        //        if (!CombatSystem.CanAttack(NodeRef)) { return; }

        //        //Doesn't Have a Challenger
        //        gm.SetChallenger(this);
        //    }
        //}
    }
    
    

    void MouseDown_Fortify()
    {

        if (GameMaster.GetInstance().IsNetworked)
        {
            // Check if the player does not match the current player
            if (!ClientPlayerController.IsCurrentPlayer(gm))
            {
                return;
            }
        }
        if (Player == null)
        {
            gm.FortifyTile(this);
        }

        //if (gm.GetPlayerTurn() == Player.playerID)
        //{
        //    if (gm.HasChallengerCheck())
        //    {
        //        // Do not let
        //        if (gm.GetChallenger() == this)
        //        {
        //            // Prevent the deselection of fortification during the attack phase
        //            if (gm.GetState() != GameState.Attack)
        //            {
        //                gm.ReleaseChallenger();
        //                MapDrawSystem.CancelArrow();
        //                ConfirmUI.CancelConfirm();
        //            }
        //        }
        //        else 
        //        {
        //            MapSystem.Board board = BoardManager.instance.GetBoard();
        //            // Check if the defender is connected to the challenger
        //            if (!board.GetConnectedTiles(NodeRef.ID).Contains(gm.GetChallenger().NodeRef))
        //            {
        //                // Don't do anything if it does not
        //                return;
        //            }
                    
        //            //NodeRef.TransferUnits(gm.GetChallenger().NodeRef, 1);
        //            ////gm.GetChallenger().Units = gm.GetChallenger().NodeRef.UnitCount;
        //            ////Units = NodeRef.UnitCount;
        //            //gm.ReleaseChallenger();
        //            // Mark this tile as the challenger
        //            gm.SetDefender(this);

        //            // Draw the arrow displaying who is attacking on the map
        //            MapDrawSystem.SpawnArrow(gm.GetChallenger().NodeRef.Position, NodeRef.Position);

        //            // Pull up the confirm menu
        //            ConfirmUI.BeginConfirm("Fortify", ConfirmUI.ConfirmType.Fortify, gm.GetChallenger().NodeRef, NodeRef);
        //        }
        //    }
        //    else
        //    {
        //        // Only allow if the unit count is greater than 1
        //        if(NodeRef.UnitCount <= 1) { return; }

        //        //Doesn't Have a Challenger
        //        gm.SetChallenger(this);
        //    }
        //}
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //Gizmos.DrawSphere(Center, 0.5f);
    }

    public void SetOwner(Player newOwner)
    {

        NodeRef.ChangeOwner(newOwner);
    }
}
