using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinentRenderer : MonoBehaviour
{
    SpriteRenderer Render;
    Texture2DArray TileTextures;
    Texture2D A;
    // Start is called before the first frame update
    void Start()
    {

        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();


        SpriteRenderer C = sprites[1];
        Texture2D t1 = C.sprite.texture;
        Color[] PixelColors = new Color[t1.GetPixels().Length];


        Debug.Log("OwO", C.gameObject);
        A = new Texture2D(t1.width, t1.height, t1.format, false); 

        Render = GetComponent<SpriteRenderer>();
        //    A.s
     //   TileTextures = new Texture2DArray(t1.width, t1.height, transform.childCount, t1.graphicsFormat, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);

        
        for (int i = 1; i < transform.childCount+1; i++)
        {
            Debug.Log(i,sprites[i].gameObject);
            Texture2D B = sprites[i].sprite.texture;
            Color[] pixels = B.GetPixels();
            for (int j = 0; j < pixels.Length; j++)
            {
                if (pixels[j] != Color.black)
                   
                    PixelColors[j] = Color.white;
            }
            

            
        }
        A.SetPixels(PixelColors);
        A.Apply();


        Debug.Log(A);
        Render.sprite =Sprite.Create(A,sprites[1].sprite.rect,new Vector2(-t1.width,-t1.height)) ;// Resources.Load(;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
