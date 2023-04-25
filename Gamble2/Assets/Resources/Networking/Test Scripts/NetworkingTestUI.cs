using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class NetworkingTestUI : MonoBehaviour
{
    [SerializeField] Button hostButton;
    [SerializeField] Button clientButton;


    private void Awake()
    {
        hostButton.onClick.AddListener(()=>{
            NetworkManager.Singleton.StartHost();
        });

        clientButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
