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

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;

    [Tooltip("The number of nodes on this graph")]
    [SerializeField] int numberOfNodes;

    [SerializeField] List<NodeConnectionPair> connections;

    [SerializeField] List<Vector3> nodePositions;

    [SerializeField] [Range(.5f, 10.0f)] float nodeCenterRadius = 1.0f;

    // The board within the scene
    Board board;

    // Awake is called before the first frame update
    void Awake()
    {
        instance = this;

        // Create the board
        board = new Board();

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
    }

    private void OnDrawGizmos()
    {
        if (board == null) { return; }

        Gizmos.color = Color.green;
        // Draw dots for all of the position of nodes on the board
        for(int i=0; i < board.Count; ++i)
        {
            BoardTile tile = (BoardTile)board[i];
            Gizmos.DrawWireSphere(tile.Position, nodeCenterRadius) ;
        }
    }

    public Board GetBoard()
    {
        return board;
    }
}
