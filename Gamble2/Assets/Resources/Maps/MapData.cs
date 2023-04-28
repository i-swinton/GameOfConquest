using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Map")]

public class MapData : ScriptableObject
{
    public List<MapContinent> Contenents;


    [Tooltip("A list of pairs to be connected to each other regardless of location")]
    public List<NodeConnectionPair> connectionPairs;
    public List<NodeConnectionPairNamed> namedPairs;

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