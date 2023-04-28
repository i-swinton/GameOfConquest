using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberScroll : MonoBehaviour
{
    // ------------------------------------------ Variables -------------------------------------------------
    List<string> options;
    int index;

    [SerializeField]
    TMPro.TextMeshProUGUI displayText;

    //------------------------------------- Public Functions ------------------------------------------------
    //private void Awake()
    //{
    //    options = new List<string>();
    //}



    public void Initialize(List<string> opts, int startingIndex)
    {
        index = startingIndex;

        if (options == null) { options = new List<string>(); }

        // Reset our options
        options.Clear();
        options.AddRange(opts);


            // Set our display value
            displayText.text = options[index];

    }

    public void Step(int direction)
    {
        index = (index + direction);
        
        if(options.Count ==0) { throw new System.Exception("Options is empty."); }

        // Clamp the value
        while (index >= options.Count)
        {
            index -= options.Count;
        }
        while (index < 0)
        {
            if(options.Count == 0) { index = 0; }
            index += options.Count;
        }

        // Set the display value
        displayText.text = options[index];
    }
    //------------------------------------ Preview Functions ---------------------------------------------
    public int PreviewInt()
    {
        return int.Parse(displayText.text);
    }
    public string PreviewString()
    {
        return displayText.text;
    }
}
