using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinentRenderer : MonoBehaviour
{
    public Color OwnerlessColor = Color.clear;

    MapSystem.Continent Continent;

    SpriteRenderer Render;
    Texture2D CombineText;

    Rect Rect;
    // Start is called before the first frame update
    public void LoadContinent(MapSystem.Continent continent)
    {
        //Sets continent Ref
        Continent = continent;

        Render = GetComponent<SpriteRenderer>();

        //Gets sprites renders of tiles
        SpriteRenderer[] sprites;
        {
            MapTileRender[] tiles = GetComponentsInChildren<MapTileRender>();
            sprites = new SpriteRenderer[tiles.Length];
            for (int i = 0; i < tiles.Length; i++)
            {
                sprites[i] = tiles[i].GetComponent<SpriteRenderer>();
            }
        }

        //Intilises variables
        Texture2D baseText = sprites[0].sprite.texture;
        Color[] PixelColors = new Color[baseText.GetPixels().Length];
        CombineText = new Texture2D(baseText.width, baseText.height, baseText.format, false); 

        //Paints continent outline
        for (int i = 0; i < transform.childCount; i++)
        {
            Texture2D paintingText = sprites[i].sprite.texture;
            Color[] pixels = paintingText.GetPixels();
            for (int j = 0; j < pixels.Length; j++)
            {
                if (pixels[j] != Color.black)
                   
                    PixelColors[j] = Color.white;
            }
        }

        //Applys colors
        CombineText.SetPixels(PixelColors);
        CombineText.Apply();

        //Creates sprite
        Rect = sprites[0].sprite.rect;
        Render.sprite = Sprite.Create(CombineText, Rect, Vector2.one * 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Continent.LastOwningPlayer != null)
            Render.color = Continent.LastOwningPlayer.playerColor;
        else
            Render.color = OwnerlessColor;
    }
}
