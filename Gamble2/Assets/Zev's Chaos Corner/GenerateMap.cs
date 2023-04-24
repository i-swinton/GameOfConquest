using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{

    public Sprite[] Tiles;

    public GameObject TilePrefab;
    // Start is called before the first frame update
    void Start()
    {
        GenBoard();
    }

    public void GenBoard()
    {
        for (int i = 0; i < Tiles.Length; i++)
        {
            GameObject tile = Instantiate(TilePrefab,transform);
            tile.GetComponent<SpriteRenderer>().sprite = Tiles[i];
            tile.AddComponent<PolygonCollider2D>();
            tile.GetComponent<MapTile>().NodeRef = (MapSystem.BoardTile)BoardManager.instance.GetBoard()[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
