using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="MapDataList",menuName ="")]
public class MapDataList : ScriptableObject
{
    public static MapDataList instance;

    [SerializeField] List<MapData> maps;

    public static MapData Get(int i)
    {

        if(instance == null)
        {
            // Load in the resource data
            instance = Resources.Load("Maps/Maps/MapDataList") as MapDataList;
        }
            // If the maps are too little, throw an error
            if( instance.maps.Count <= i)
            {
                throw new System.IndexOutOfRangeException($"No map found with ID {i}");
            }
        return instance.maps[i];
        
    }
}
