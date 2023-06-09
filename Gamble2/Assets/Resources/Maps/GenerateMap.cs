using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    [SerializeField] MapData Map;


    public GameObject ContinentPrefab;
    public GameObject TilePrefab;

    public ContactFilter2D ConnectionFilter;

    public List<MapTile> mapTiles;


   public static GenerateMap instance;


    private void Awake()
    {
        mapTiles = new List<MapTile>();
        instance = this;
    }

    public void SetMap(MapData map)
    {
        Map = map;
    }

    public static MapTile GetTile(int id)
    {
        return instance.mapTiles[id];
    }

    public static MapTile GetTile(MapSystem.BoardTile tile)
    {
        for(int i =0; i < instance.mapTiles.Count; ++i)
        {
            // If the instance matches, return
            if(instance.mapTiles[i].NodeRef == tile)
            {
                return instance.mapTiles[i];
            }
        }

        return null;
    }

    // Start is called before the first frame update

    public void GenBoard()
    {
        // Create the board
        BoardManager.instance.MakeBoard(Map.Contenents);

        MapSystem.Board board = BoardManager.instance.GetBoard();

        List<MapTile> spawnedTiles = new List<MapTile>();

        for (int i = 0; i < Map.Contenents.Count; i++)
        {
            GameObject continent = Instantiate(ContinentPrefab, transform);
            //Generates tiles within a continent
            for (int j = 0; j < Map.Contenents[i].Tiles.Length; j++)
            {
                GameObject tile = Instantiate(TilePrefab, continent.transform);
                tile.GetComponent<SpriteRenderer>().sprite = Map.Contenents[i].Tiles[j].Image;
                tile.AddComponent<PolygonCollider2D>();

                // Generate the tile
                tile.GetComponent<MapTile>().GenTile(
                    board[board.FindContinent(i).Tiles[j]]
                    ,Map.Contenents[i].Tiles[j].Name);

                // Set the tile's position (for debug references)
                tile.GetComponent<MapTile>().NodeRef.MoveTo(tile.GetComponent<PolygonCollider2D>().bounds.center);


                tile.name = tile.GetComponent<MapTile>().NodeRef.Name + tile.GetComponent<MapTile>().NodeRef.ID;
                tile.GetComponent<MapTileRender>().LoadTile();

                // Add the tiles to the list
                spawnedTiles.Add(tile.GetComponent<MapTile>());

                tile.GetComponent<MapTile>().SetIndex(spawnedTiles.Count-1);
            }

            continent.GetComponent<ContinentRenderer>().LoadContinent(board.FindContinent(i));
            continent.name = Map.Contenents[i].Name;
            //Adds continent to board
            //BoardManager.instance.GetBoard().AddContinent( new MapSystem.Continent(tiles, Contenents[i].Name, Contenents[i].bonus));
            
        }

        // Set the map tiles for future reference
        mapTiles = spawnedTiles;

        GenerateConnections(board, spawnedTiles);
        ForceConnections(board, Map.namedPairs);

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

                GetTile(board.Find(pair.node1)).Render.GenConnection (GetTile(board.Find(pair.node2)), pair.RenderConnection);
            }
        }
    }

    public void GenerateConnections(MapSystem.Board board, List<MapTile> tiles )
    {
        for(int i=0; i < tiles.Count; ++i)
        {
            // Grab all of the map tiles connected to this one
            List<MapTile> connectedNodes = FindConnections(tiles[i]);


            // Loop through the nodes to connect them
            foreach(MapTile node in connectedNodes)
            {
                board.Connect(tiles[i].NodeRef.ID, node.NodeRef.ID);

                tiles[i].Render.GenConnection(node, ConnectionRender.Adjecent);
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
