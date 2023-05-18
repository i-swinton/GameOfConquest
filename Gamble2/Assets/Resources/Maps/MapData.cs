using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CreateAssetMenu(fileName = "MapData", menuName = "Map")]

public class MapData : ScriptableObject
{
    public Texture2D Image;

    [Multiline()]
    public string Descrition;

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

#if UNITY_EDITOR

[CustomEditor(typeof(MapData))]
public class MapDataEditor : Editor
{

    public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
    {
        MapData mapData = target as MapData;
        if (mapData == null || mapData.Image == null)
            return null;

        Texture2D cache = new Texture2D(width, height);

        Texture2D preview = AssetPreview.GetAssetPreview(mapData.Image);

        EditorUtility.CopySerialized(preview, cache);

        return cache;

       // return base.RenderStaticPreview(assetPath, subAssets, width, height);
    }
}
#endif