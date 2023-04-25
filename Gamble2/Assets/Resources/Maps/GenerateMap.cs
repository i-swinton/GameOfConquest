using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    
    [SerializeField] List<MapContinent> Contenents;
    public GameObject TilePrefab;

    public ContactFilter2D ConnectionFilter;

    [Tooltip("A list of pairs to be connected to each other regardless of location")]
    [SerializeField] List<NodeConnectionPair> connectionPairs;
    [SerializeField] List<NodeConnectionPairNamed> namedPairs;

    // Start is called before the first frame update
    void Start()
    {
        GenBoard();
    }

    public void GenBoard()
    {
        // Create the board
        BoardManager.instance.MakeBoard(Contenents);

        MapSystem.Board board = BoardManager.instance.GetBoard();

        List<MapTile> spawnedTiles = new List<MapTile>();

        for (int i = 0; i < Contenents.Count; i++)
        {


            //Generates tiles within a continent
            for (int j = 0; j < Contenents[i].Tiles.Length; j++)
            {
                GameObject tile = Instantiate(TilePrefab, transform);
                tile.GetComponent<SpriteRenderer>().sprite = Contenents[i].Tiles[j].Image;
                tile.AddComponent<PolygonCollider2D>();
                // Generate the tile
                tile.GetComponent<MapTile>().GenTile(
                    board[board.FindContinent(i).Tiles[j]]
                    ,Contenents[i].Tiles[j].Name);

                // Set the tile's position (for debug references)
                tile.GetComponent<MapTile>().NodeRef.MoveTo(tile.GetComponent<PolygonCollider2D>().bounds.center);


                // Add the tiles to the list
                spawnedTiles.Add(tile.GetComponent<MapTile>());
            }

            //Adds continent to board
            //BoardManager.instance.GetBoard().AddContinent( new MapSystem.Continent(tiles, Contenents[i].Name, Contenents[i].bonus));
            
        }

        GenerateConnections(board, spawnedTiles);
        ForceConnections(board, namedPairs);
    }

    public void ForceConnections(MapSystem.Board board, List<NodeConnectionPairNamed> pairs)
    {
        // Loop through all of the pairs and connect them.
        foreach (NodeConnectionPairNamed pair in pairs)
        {
            if (board.Contains(pair.node1) && board.Contains(pair.node2))
            {
                // Find and connect the two nodes
                board.Connect(board[pair.node1].ID, board[pair.node2].ID);
            }
        }
    }

    public void GenerateConnections(MapSystem.Board board, List<MapTile> tiles )
    {
        for(int i=0; i < tiles.Count; ++i)
        {
            // Grab all of the map tiles connected to this one
            var connectedNodes = FindConnections(tiles[i]);

            // Loop through the nodes to connect them
            foreach(var node in connectedNodes)
            {
                board.Connect(tiles[i].NodeRef.ID, node.NodeRef.ID);
            }
        }
    }

    private List<MapTile> FindConnections(MapTile tile)
    {
        PolygonCollider2D Collider = tile.GetComponent<PolygonCollider2D>();

        // Tiles to be connected to tile
        List<MapTile> tiles = new List<MapTile>();

        List<Collider2D> Results = new List<Collider2D>();

        Vector3 Center = Collider.bounds.center;
        Collider.OverlapCollider(ConnectionFilter, Results);
        foreach (Collider2D item in Results)
        {

            Vector3 point = item.ClosestPoint(Center);


            float angle = Vector3.SignedAngle(point, Center, Vector3.forward);
            angle = Mathf.Rad2Deg * Mathf.Asin((point - Center).normalized.y);
            Debug.Log($"{(point - Center).normalized.y}, {angle}, {item.GetComponent<MapTile>().NameDisplay.text}");

            angle %= 90;
            angle = Mathf.Abs(angle);

            if (angle > 22 && angle < 70)
            { }//    continue;
            else
            {
                // Check if it is a tile
                MapTile tileToAdd = item.GetComponent<MapTile>();
                if (tileToAdd)
                {
                    // Add to list
                    tiles.Add(tileToAdd);
                }
                //points.Add(item.ClosestPoint(Center));
                //points.Add(item.bounds.center);
            }
        }

        return tiles;

    }

}
[System.Serializable]
public class MapContinent
{
    public string Name;
    public List<BonusBase> bonus;
    public TileData[] Tiles;
}

[System.Serializable]
public struct TileData
{
    public string Name;
    public Sprite Image;
  //  Sprite this[int i = 0] { get { return Image; } }
}