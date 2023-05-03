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
        Debug.Log("Current end point:" + NetworkManager.GetComponent<UnityTransport>().ConnectionData.ServerEndPoint);
        NetworkManager.GetComponent<UnityTransport>().ConnectionData.Address = addressText.text.Substring(0,addressText.text.Length-1);
        Debug.Log(portText.text.Substring(0, portText.text.Length - 1));
        NetworkManager.GetComponent<UnityTransport>().ConnectionData.Port = ushort.Parse(portText.text.Substring(0,portText.text.Length-1));

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
        NetworkPlayerDataCarrier.Instance.onLoadInGame += BeginGameScene;
        
        // Load in the game settings
        //NetworkPlayerDataCarrier.LoadGameData_ServerRPC(MatchSettingsPanels.GetGameMode(), MatchSettingsPanels.GetSettings());
        LoadGameData_ServerRPC(MatchSettingsPanels.GetGameModeIndex(), 
           new NetworkSystem.GameSettingStruct( MatchSettingsPanels.GetGameSettingsList()));

        //NetworkManager.SceneManager.LoadScene(gameplayScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    [ServerRpc]
    public  void LoadGameData_ServerRPC(int modeIndex, NetworkSystem.GameSettingStruct settings)
    {
        // Broadcast the data to all of the clients
        LoadGameData_ClientRPC(modeIndex, settings);
    }

    [ClientRpc]
    public  void LoadGameData_ClientRPC(int modeIndex, NetworkSystem.GameSettingStruct settings)
    {
        //DebugNetworklLog.Log("Setting data on "+instance)
        NetworkPlayerDataCarrier.LoadGameData(GameModeList.GetGameMode(modeIndex), new GameSettings(settings));
    }

    public void BeginGameScene()
    {
        if(IsHost)
        {
            NetworkManager.SceneManager.LoadScene(gameplayScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

   
}
