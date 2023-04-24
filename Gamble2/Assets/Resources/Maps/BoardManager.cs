using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapSystem;

[System.Serializable]
public class NodeConnectionPair
{
    public int node1;
    public int node2;
}

[System.Serializable]
public class BonusEditorList
{
    public List<BonusBase> bonuses;
}

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;

    [Tooltip("The number of nodes on this graph")]
    [SerializeField] int numberOfNodes;

    [SerializeField] List<NodeConnectionPair> connections;

    [SerializeField] List<Vector3> nodePositions;

    [SerializeField] [Range(.5f, 10.0f)] float nodeCenterRadius = 1.0f;

    [SerializeField] List<Continent> continents;

    [SerializeField] List<BonusEditorList> tileBonuses;

    // The board within the scene
    Board board;

    // Awake is called before the first frame update
    void Awake()
    {
        instance = this;

        // Create the board
        board = new Board();
        
        // Create tile bonus array
        //tileBonuses = new List<List<BonusBase>>();

        // Create all of the nodes for the board
        for(int i =0; i < numberOfNodes; ++i)
        {
            // Fill the board with postions listed in the editor
            Vector3 pos = nodePositions.Count > i ? nodePositions[i] : Vector3.zero;

            board.MakeNode(pos);
        }

        // Create all of the  connections
        foreach(NodeConnectionPair pair in connections)
        {
            board.Connect(pair.node1, pair.node2);
        }

        // Add in all of the continents
        foreach (Continent c in continents)
        {
            board.AddContinent(c);

        }

        // Loop through all of the tiles...
        for(int i=0; i < tileBonuses.Count; ++i)
        {
            // Apply the bonuses to tiles
            foreach(BonusBase bonus in tileBonuses[i].bonuses)
            {
                board[i].ApplyBonus(bonus);
            }
        }

    }

    private void Update()
    {
        // Populate
        if(Input.GetKeyDown(KeyCode.P))
        {
            for(int i =0; i < board.Count; ++i)
            {
                board[i].AddUnits(new Unit(10));
            }
        }
        // Battle single
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Perform combat between the two board pieces
            CombatSystem.BattleTiles(board[0], board[1], Combat.CombatRollType.Single, out int atkLoss, out int defLoss);

            Debug.Log("Attack Losses: " + atkLoss + ", Def Loss: " + defLoss);
            Debug.Log(board[0].UnitCount + " on tile 1");
            Debug.Log(board[1].UnitCount + " on tile 2");
        }
        // Battle single
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // Perform combat between the two board pieces
            CombatSystem.BattleTiles(board[0], board[1], Combat.CombatRollType.Double, out int atkLoss, out int defLoss);

            Debug.Log("Attack Losses: " + atkLoss + ", Def Loss: " + defLoss);
            Debug.Log(board[0].UnitCount + " on tile 1");
            Debug.Log(board[1].UnitCount + " on tile 2");
        }
        // Battle single
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // Perform combat between the two board pieces
            CombatSystem.BattleTiles(board[0], board[1], Combat.CombatRollType.Triple, out int atkLoss, out int defLoss);

            Debug.Log("Attack Losses: " + atkLoss + ", Def Loss: " + defLoss);
            Debug.Log(board[0].UnitCount + " on tile 1");
            Debug.Log(board[1].UnitCount + " on tile 2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            // Perform combat between the two board pieces
            CombatSystem.BattleTiles(board[0], board[1], Combat.CombatRollType.Blitz, out int atkLoss, out int defLoss);

            Debug.Log("Attack Losses: " + atkLoss + ", Def Loss: " + defLoss);
            Debug.Log(board[0].UnitCount + " on tile 1");
            Debug.Log(board[1].UnitCount + " on tile 2");
        }
    }

    private void OnDrawGizmos()
    {
        if (board == null) { return; }

        Gizmos.color = Color.green;
        // Draw dots for all of the position of nodes on the board
        for(int i=0; i < board.Count; ++i)
        {

            BoardTile tile = board[i];
            Gizmos.DrawWireSphere(tile.Position, nodeCenterRadius) ;
        }
    }

    public Board GetBoard()
    {
        return board;
    }
}
