using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCountryRender : MonoBehaviour
{
    SpriteRenderer Renderer;

    public Color Hilighted = Color.gray;
    public Color Default = Color.white;
    // Start is called before the first frame update
    void Awake()
    {
        Renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHeilight(bool value)
    {
        if (value)
        {
            Renderer.color = Hilighted;
        }
        else
            Renderer.color = Default;
    }
    public void DebugLog()
    {
        Debug.Log("Test", this);
    }
}
