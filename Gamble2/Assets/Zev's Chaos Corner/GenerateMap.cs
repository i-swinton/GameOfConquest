using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    
    [SerializeField] List<MapContinent> Contenents;
    public GameObject TilePrefab;
    // Start is called before the first frame update
    void Start()
    {
        GenBoard();
    }

    public void GenBoard()
    {
        for (int i = 0; i < Contenents.Count; i++)
        {
            List<int> tiles = new List<int>();

            //Generates tiles within a continent
            for (int j = 0; j < Contenents[i].Tiles.Length; j++)
            {
                GameObject tile = Instantiate(TilePrefab, transform);
                tile.GetComponent<SpriteRenderer>().sprite = Contenents[i].Tiles[j].Image;
                tile.AddComponent<PolygonCollider2D>();
                tile.GetComponent<MapTile>().GenTile(BoardManager.instance.GetBoard()[j],Contenents[i].Tiles[j].Name);
                tiles.Add(j);
            }

            //Adds continent to board
            BoardManager.instance.GetBoard().AddContinent( new MapSystem.Continent(tiles, Contenents[i].Name, Contenents[i].bonus));

        }


    }

    // Update is called once per frame
    void Update()
    {
        
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