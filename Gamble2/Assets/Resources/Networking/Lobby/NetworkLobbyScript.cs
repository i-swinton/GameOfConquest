using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;

public class NetworkLobbyScript : NetworkBehaviour
{
    [Header("Referneces")]
    [SerializeField] TMPro.TextMeshProUGUI addressText;
    [SerializeField] TMPro.TextMeshProUGUI portText;

    [SerializeField] GameObject searchForGamePanel;
    [SerializeField] GameObject hostForGamePanel;

    [Header("Host Panel References")]
    [SerializeField] TMPro.TextMeshProUGUI[] playerTexts;
  

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Connect()
    {
        NetworkManager.GetComponent<UnityTransport>().ConnectionData.Address = addressText.text;
        NetworkManager.GetComponent<UnityTransport>().ConnectionData.Port = ushort.Parse(portText.text);

        // Connect to the server
        //NetworkManager.Singleton.StartClient();
        try
        {
            StartClient();

            searchForGamePanel.SetActive(false);
            hostForGamePanel.SetActive(true);
        }catch(System.Exception err)
        {
            DebugNetworklLog.Log(err.Message);
        }
    }

    public void Host()
    {
        searchForGamePanel.SetActive(false);
        hostForGamePanel.SetActive(true);

        StartHost();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }

    
}
