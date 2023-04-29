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

//Show blizards and capitals
public class MapTileRender : MonoBehaviour
{
    MapTile Tile;
    SpriteRenderer Renderer;
    [SerializeField] Material Mat;
    public SelectState _State;

    Vector3 Center;
    bool otherSel;

    [SerializeField] List<BonusRender> DisplayFeatures;

    [System.Serializable]
    struct BonusRender
    {
        public BonusBase Bonus;
        public GameObject Display;
        public static bool operator ==(BonusRender b1, BonusBase b2)
        {
            return b1.Bonus.GetType() == b2.GetType();
        }
        public static bool operator !=(BonusRender b1, BonusBase b2)
        {
            return b1.Bonus.GetType() != b2.GetType();
        }

    }

    public SelectState State{
        get {
            return _State;
        }
        set
        {
            _State = value;
            Mat.SetFloat("_State", (float)State);
        }
    }

    // Start is called before the first frame update
    public void LoadTile()
    {
        Tile = GetComponent<MapTile>();

        Center = Tile.Center;// GetComponent<PolygonCollider2D>().bounds.center;


        //Creates a clone of the material
        Mat = new Material(Mat);

        Renderer = GetComponent<SpriteRenderer>();
        Renderer.material = Mat;


        Tile.NodeRef.onBindBonus += DisplayBonus;
    }

    public void Select()
    {
        // Do not do anything if already selected
        if(State == SelectState.Selected) { return; }


        GameMaster.AddAction(new Actions.Scale(Vector3.one * 1.4f, Vector3.one, 
            gameObject, 0.2f,0.0f, Actions.ActionType.NoGroup, false, Actions.EaseType.Linear ));

        State = SelectState.Selected;
    }
    public void CanSelect(bool OtherSelected)
    {
        // Do not do anything if already in the correct can select state
        if(State == SelectState.CanSelect && otherSel == OtherSelected) { return; }


        GameMaster.AddAction(new Actions.Scale(Vector3.one * 1.1f, Vector3.one, 
            gameObject, 0.2f,0.0f, Actions.ActionType.NoGroup, false, Actions.EaseType.Linear ));

        otherSel = OtherSelected;
        State = SelectState.CanSelect;

    }
    public void Deselect()
    {
        // Do not do anything if already deselected
        if(State== SelectState.CannotSelect) { return; }

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

    public void DisplayBonus(BonusBase bonus)
    {
        Vector3 scale = transform.localScale;
        transform.localScale = Vector3.one;

        foreach (BonusRender item in DisplayFeatures)
        {
            if(item == bonus)
            {
                GameObject displayFeature = Instantiate(item.Display, transform);
                displayFeature.transform.position= Center + Vector3.back;
            }
        }
        transform.localScale = scale;
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

