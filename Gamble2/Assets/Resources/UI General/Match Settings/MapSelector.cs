using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class MapSelector : MonoBehaviour
{
    static MapSelector instance;

    [Header("Pannels")]

    [SerializeField] RawImage MapDisplay;
    [SerializeField] TextMeshProUGUI NameDispaly;
    [SerializeField] TextMeshProUGUI DescritionDisplay;

    [Header("Data")]
    [SerializeField] List<MapData> Maps;


    int SelectedMap = 0;
    float Height;

    public void SetValue(int value)
    {
        
        SelectedMap = value;

        //Sets Image
        MapDisplay.texture = Maps[SelectedMap].Image;
        float aspect =(float)Maps[SelectedMap].Image.height / Maps[SelectedMap].Image.width;

        if(aspect <= 1)
        {
            MapDisplay.rectTransform.sizeDelta = new Vector2(MapDisplay.rectTransform.sizeDelta.x, Height * aspect); 
        }
        else
        {
            MapDisplay.rectTransform.sizeDelta = new Vector2(Height * aspect, MapDisplay.rectTransform.sizeDelta.y);
        }

        //Sets Text
        NameDispaly.text = Maps[SelectedMap].name;
        DescritionDisplay.text = Maps[SelectedMap].Descrition;
    }

    public void AdjustValue(int delta)
    {
        int newValue =SelectedMap+ delta;

        if (newValue >= Maps.Count)
            newValue = 0;

        if (newValue < 0)
            newValue = Maps.Count - 1;

        SetValue(newValue);
    }

    public static MapData GetValue()
    {
        return instance.Maps[instance.SelectedMap];
    }
    // Start is called before the first frame update
    void Start()
    {
        Height = MapDisplay.rectTransform.rect.height;
        SetValue(0);
    }

    // Update is called once per frame
    void Awake()
    {
        instance = this;
    }
}
