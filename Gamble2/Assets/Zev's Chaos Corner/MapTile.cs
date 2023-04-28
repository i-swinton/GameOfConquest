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

    // Start is called before the first frame update
    public void GenTile(MapSystem.BoardTile nodeRef, string name)
    {
        NodeRef = nodeRef;
        Name = name;

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
        switch (gm.GetState())
        {
            case GameState.Claim: MouseDown_Claim(); break;
            case GameState.Reinforce: MouseDown_Reinforce(); break;
            case GameState.Draft: MouseDown_Draft(); break;
            case GameState.Attack: MouseDown_Attack(); break; 
            case GameState.Fortify: MouseDown_Fortify();  break;
        }
    }

    void MouseDown_Claim()
    {
        if (Player == null)
        {
            NodeRef.ChangeOwner( gm.GetPlayer());
            Player.draftTroop--;
            NodeRef.AddUnits(new Unit(1));
            gm.EndTurn();
        }
    }
    
    void MouseDown_Reinforce()
    {
        if (gm.GetPlayerTurn() == Player.playerID)
        {
            NodeRef.Fortify(1);
            Player.draftTroop--;
            gm.EndTurn();
        }
    }
    
    void MouseDown_Draft()
    {
        if (gm.GetPlayerTurn() == Player.playerID)
        {
            NodeRef.Fortify(1);
            Player.draftTroop--;
            
            if (Player.draftTroop <= 0)
                gm.EndTurn();
        }
    }
    
    void MouseDown_Attack()
    {
        
        if (gm.HasChallengerCheck())
        {
            //Has a Challenger
            if (gm.GetChallenger() == this)
            {
                gm.ReleaseChallenger();
            }
            else
            {
                // Battle two challengers
                if (gm.GetChallenger().Player.playerID != Player.playerID)
                {
                    if (gm.GetChallenger().NodeRef.Neighbors.Contains(NodeRef))
                    {
                        // Mark this tile as the challenger
                        gm.SetDefender(this);

                        // Draw the arrow displaying who is attacking on the map
                        MapDrawSystem.SpawnArrow(gm.GetChallenger().NodeRef.Position, NodeRef.Position);

                        // Pull up the confirm menu
                        ConfirmUI.BeginConfirm($"Attack {NodeRef.Name}?", ConfirmUI.ConfirmType.Battle,gm.GetChallenger().NodeRef);

                    }
                }
            }
            
        }
        else
        {
            if (gm.GetPlayerTurn() == Player.playerID)
            {
                //Doesn't Have a Challenger
                gm.SetChallenger(this);
            }
        }
    }
    
    

    void MouseDown_Fortify()
    {
        if (gm.GetPlayerTurn() == Player.playerID)
        {
            if (gm.HasChallengerCheck())
            {
                if (gm.GetChallenger() == this)
                {
                    gm.ReleaseChallenger();
                }
                else
                {
                    NodeRef.TransferUnits(gm.GetChallenger().NodeRef, 1);
                    //gm.GetChallenger().Units = gm.GetChallenger().NodeRef.UnitCount;
                    //Units = NodeRef.UnitCount;
                    gm.ReleaseChallenger();
                }
            }
            else
            {
                //Doesn't Have a Challenger
                gm.SetChallenger(this);
            }
        }
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
