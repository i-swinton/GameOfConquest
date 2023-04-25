using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkConsole : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI text;

    [SerializeField] GameObject content;

    public static NetworkConsole instance;

    

    private void Awake()
    {
        instance = this;
    }

    public void SetText(string text)
    {
        // Set the text of the text log
        this.text.text = text;
    }

    public void Hide()
    {
        content.SetActive(false);
    }

    public void Show()
    {
        content.SetActive(true);
    }

}
