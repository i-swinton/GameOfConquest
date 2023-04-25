using System;
using System.Collections;
using System.Collections.Generic;
using Combat;
using MapSystem;
using UnityEngine;
using TMPro;
public class MapTile : MonoBehaviour
{
    public Player Player = null;
    private string Name;

    public int Units = 1;


    public MapSystem.BoardTile NodeRef;
    PolygonCollider2D Colid;
    public Vector3 Center = new Vector3(1,1,1);

    public Transform UIDisplays;
    public TextMeshPro NameDisplay;
    public TextMeshProUGUI UnitsDisplay;

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
            NameDisplay.text = Units.ToString();
        }
    }

    private void OnMouseDown()
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
            Player = gm.GetPlayer();
            Player.draftTroop--;
            NodeRef.AddUnits(new Unit(1));
            Units = 1;
            gm.EndTurn();
        }
    }
    
    void MouseDown_Reinforce()
    {
        if (gm.GetPlayerTurn() == Player.playerID)
        {
            Units++;
            NodeRef.Fortify(1);
            Player.draftTroop--;
            gm.EndTurn();
        }
    }
    
    void MouseDown_Draft()
    {
        if (gm.GetPlayerTurn() == Player.playerID)
        {
            Units++;
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
                if (gm.GetChallenger().Player.playerID != Player.playerID)
                {
                    int AtkUnitsLost = 0, DefUnitsLost = 0;
                    CombatSystem.BattleTiles(gm.GetChallenger().NodeRef, this.NodeRef, CombatRollType.Single, out AtkUnitsLost, out DefUnitsLost);
                    Units -= DefUnitsLost;
                    gm.GetChallenger().Units -= AtkUnitsLost;
                    

                    if (Units <= 0)
                    {
                        Player = gm.GetChallenger().Player;
                        NodeRef.Fortify(3);
                        Units = 3;
                        
                    }
                    
                    gm.ReleaseChallenger();
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
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        //Gizmos.DrawSphere(Center, 0.5f);
    }
}
