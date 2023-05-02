using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;


public class NetworkSpawnControlPanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("Current end point:" + NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.ServerEndPoint);

    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        Debug.Log("Current end point:" + NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.ServerEndPoint);

    }
}
