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


    public void Select()
    {
        GameMaster.AddAction(new Actions.Scale(Vector3.one * 1.4f, Vector3.one, 
            gameObject, 0.2f,0.0f, Actions.ActionType.NoGroup, false, Actions.EaseType.Linear ));

        State = SelectState.Selected;
    }
    public void CanSelect(bool OtherSelected)
    {
        
        GameMaster.AddAction(new Actions.Scale(Vector3.one * 1.1f, Vector3.one, 
            gameObject, 0.2f,0.0f, Actions.ActionType.NoGroup, false, Actions.EaseType.Linear ));


        State = SelectState.CanSelect;

    }
    public void Deselect()
    {
        GameMaster.AddAction(new Actions.Scale(Vector3.one, Vector3.one * 1.1f, gameObject, 0.2f,
            0.0f, Actions.ActionType.Tiles, false, Actions.EaseType.Linear));

        // Set the new state
        State = SelectState.CannotSelect;

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
            Mat.SetFloat("_Hovering", 1);
        }
        else
        {
            Mat.SetFloat("_Hovering", 0);
        }
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
