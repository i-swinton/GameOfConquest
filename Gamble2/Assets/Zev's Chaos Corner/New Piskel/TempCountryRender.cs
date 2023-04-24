using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCountryRender : MonoBehaviour
{
    SpriteRenderer Renderer;
    public Material Mat;
    public MaterialPropertyBlock Block;
    public bool Heilight;
    public enum SelectState
    {
        CannotSelect,
        CanSelect,
        Selected,
        OtherSelected
    }
    public SelectState _State;

    public SelectState State { get{
            return _State;
        }
        set
        {
            _State = value;
            Mat.SetFloat("_State", (float)State);
            Debug.Log("State Change", this);

        }
    }
    /*
     * Cannot Select
     * Can Select
     * Selected
     * OtherSelected
     */

    public Color Hilighted = Color.gray;
    public Color Default = Color.white;
    // Start is called before the first frame update
    void Awake()
    {
        Mat = new Material(Mat);
        Block = new MaterialPropertyBlock();
        Renderer = GetComponent<SpriteRenderer>();
        Renderer.material = Mat;

        Mat.SetColor("_PlayerColor", Color.green);
      //  Renderer.SetPropertyBlock(Block);

    }

    // Update is called once per frame
    void Update()
    {

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


        for(int i=0; i < mt.NodeRef.Parent.ContinentCount; ++i)
        {
            if(mt.NodeRef.Parent.FindContinent(i).Contains(mt.NodeRef))
            {
                Debug.Log("Continent: " + mt.NodeRef.Parent.FindContinent(i).Name, this);

            }
        }

    }
}
