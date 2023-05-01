using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugNetworklLog : UIElement
{
    static DebugNetworklLog instance;

    [SerializeField]
    TMPro.TextMeshProUGUI[] textDisplays;

    [SerializeField]
    TMPro.TextMeshProUGUI title;


    private void Awake()
    {
        instance = this;
    }


    public static void Log(string text)
    {
        instance.AddLog(text);
    }
    public static void SetTitle(string title)
    {
        instance.title.text = "Log: " + title;
    }
    void AddLog(string text)
    {
        // Copy the text downwards
        for(int i= textDisplays.Length-1;  i >0; --i )
        {
            textDisplays[i].text = textDisplays[i - 1].text;
        }

        textDisplays[0].text = text;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            IsVisible = !IsVisible;
        }

    }
}
