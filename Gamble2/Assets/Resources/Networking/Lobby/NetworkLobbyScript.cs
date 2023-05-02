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
    int index;
    [SerializeField] string gameplayScene;

    [SerializeField] InputField inputField;

    private void Start()
    {
        //inputField.te
    }

    private void Update()
    {
        if (NetworkPlayerDataCarrier.Controllers.Count - 1 != index)
        {
            UpdatePlayerText();
        }
    }


    void UpdatePlayerText()
    {
        for(int i =0; i < NetworkPlayerDataCarrier.Controllers.Count; ++i)
        {
            playerTexts[i].text = $"{(IsHost?"Host ":"Client ")} "+NetworkPlayerDataCarrier.Controllers[i].InternalID;
        }

        index = NetworkPlayerDataCarrier.Controllers.Count-1;
    }
    bool ContainsPlayer()
    {
        for(int i =0; i < index; ++i)
        {
            
        }

        return false;
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

    public void LoadGame()
    {
        NetworkManager.SceneManager.LoadScene(gameplayScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
