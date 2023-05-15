using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;

public class NetworkLobbyScript : NetworkBehaviour
{
    enum LobbyState
    {
        Find,
        Host,
        Local
    }

    [Header("Referneces")]
    [SerializeField] TMPro.TextMeshProUGUI addressText;
    [SerializeField] TMPro.TextMeshProUGUI portText;

    [SerializeField] GameObject searchForGamePanel;
    [SerializeField] GameObject hostForGamePanel;
    [SerializeField] GameObject localGamePanel;

    [Header("Host Panel References")]
    [SerializeField] TMPro.TextMeshProUGUI[] playerTexts;
    [SerializeField] TMPro.TextMeshProUGUI[] pT;
    int index;
    [SerializeField] string gameplayScene;

    [SerializeField] InputField inputField;

    LobbyState state;


    private void Start()
    {
        //inputField.te
    }

    private void Update()
    {
        if (state == LobbyState.Host)
        {
            Debug.Log(index + " VS " + (NetworkPlayerDataCarrier.Controllers.Count));
            if ((NetworkPlayerDataCarrier.Controllers.Count + NetworkPlayerDataCarrier.BotCount) != index)
            {
                UpdatePlayerText();
            }
        }
        else if (state == LobbyState.Local)
        {
            if ((DataCarrier.PlayerCount + DataCarrier.BotCount) != index)
            {
                UpdatePlayerText_Local();
            }
        }
    }


    void UpdatePlayerText()
    {
        int i = 0;
        for(; i < NetworkPlayerDataCarrier.Controllers.Count; ++i)
        {
            playerTexts[i].text = $"{(IsHost?"Host ":"Client ")} "+NetworkPlayerDataCarrier.Controllers[i].InternalID;
        }


        for (; i < NetworkPlayerDataCarrier.BotCount + NetworkPlayerDataCarrier.Controllers.Count; ++i)
        {
            playerTexts[i].text = $"Bot {i}";
        }

        index = NetworkPlayerDataCarrier.Controllers.Count-1+NetworkPlayerDataCarrier.BotCount;
    }

    void UpdatePlayerText_Local()
    {
        int index = 0; 
        for(;index < DataCarrier.PlayerCount; ++index)
        {
            pT[index].text = $"Player {index}";
        }

        for(;index  < DataCarrier.BotCount+DataCarrier.PlayerCount; ++index)
        {
            pT[index].text = $"Bot {index}";
        }

        this.index = index;
    }


    public void AddBot_Net()
    {
        NetworkPlayerDataCarrier.AddBot();
    }

    public void AddBot()
    {
        DataCarrier.AddBot();
        Debug.Log("Adding bot");
    }

    public void AddPlayer()
    {

        // Add a player to the carrier
        DataCarrier.AddPlayer();
    }


    public void Host()
    {
        searchForGamePanel.SetActive(false);
        hostForGamePanel.SetActive(true);

        state = LobbyState.Host;
        GameType.isNetworked = true;

        StartHost();
    }

    public void Local()
    {
        searchForGamePanel.SetActive(false);
        hostForGamePanel.SetActive(false);
        localGamePanel.SetActive(true);

        state = LobbyState.Local;
        GameType.isNetworked = false;

        // Add player
        DataCarrier.AddPlayer();

        

    }

    public void LoadGame_Local()
    {
        // Get the index and settings
        int modeIndex = MatchSettingsPanels.GetGameModeIndex();
        var settings = new NetworkSystem.GameSettingStruct(MatchSettingsPanels.GetGameSettingsList());

        DataCarrier.Instance.onLoadInGame += BeginGameScene_Local;
        DataCarrier.LoadGameData(GameModeList.GetGameMode(modeIndex), new GameSettings(settings));
    }

    public void BeginGameScene_Local()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(gameplayScene);
    }


    #region Network Functions

    bool ContainsPlayer()
    {
        for (int i = 0; i < index; ++i)
        {

        }

        return false;
    }

    public void Connect()
    {
        Debug.Log("Current end point:" + NetworkManager.GetComponent<UnityTransport>().ConnectionData.ServerEndPoint);
        NetworkManager.GetComponent<UnityTransport>().ConnectionData.Address = addressText.text.Substring(0, addressText.text.Length - 1);
        Debug.Log(portText.text.Substring(0, portText.text.Length - 1));
        NetworkManager.GetComponent<UnityTransport>().ConnectionData.Port = ushort.Parse(portText.text.Substring(0, portText.text.Length - 1));

        // Connect to the server
        //NetworkManager.Singleton.StartClient();
        try
        {
            StartClient();

            searchForGamePanel.SetActive(false);
            hostForGamePanel.SetActive(true);

            state = LobbyState.Host;
            GameType.isNetworked = true;

        }
        catch (System.Exception err)
        {
            DebugNetworklLog.Log(err.Message);
        }
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

    #endregion
}
