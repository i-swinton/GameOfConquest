using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SelectState
{
    CannotSelect,
    CanSelect,
    Selected,
    OtherSelected
}

public class MapTileRender : MonoBehaviour
{
    SpriteRenderer Renderer;
    [SerializeField] Material Mat;
    public SelectState _State;

    public SelectState State { get{
            return _State;
        }
        set
        {
            _State = value;
            Mat.SetFloat("_State", (float)State);

        }
    }


    //Sets base color of tile to inputed color
    public void SetPlayerColor(Color color)
    {
        Mat.SetColor("_PlayerColor", color);
    }
    
    // Start is called before the first frame update
    void Awake()
    {

        //Creates a clone of the material
        Mat = new Material(Mat);

        Renderer = GetComponent<SpriteRenderer>();
        Renderer.material = Mat;


    }
    
    public void SetHeilight(bool value)
    {
        if (value)
        {
            State = SelectState.CanSelect;}
        else
        {
            State = SelectState.CannotSelect;
        }
        //Renderer.color = Default;
    }


    public void DebugLog()
    {
        State = SelectState.Selected;
        MapTile mt = GetComponent<MapTile>();


        //for(int i=0; i < mt.NodeRef.Parent.ContinentCount; ++i)
        //{
        //    if(mt.NodeRef.Parent.FindContinent(i).Contains(mt.NodeRef))
        //    {
        //        Debug.Log("Continent: " + mt.NodeRef.Parent.FindContinent(i).Name, this);

        //    }
        //}

    }
}
