using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkCommandLine : MonoBehaviour
{
    private NetworkManager netManager;

    void Start()
    {
        // Finds the net manager
        netManager = GetComponentInParent<NetworkManager>();

        if (Application.isEditor) return;

        // Gets the command line arguments
        var args = GetCommandlineArgs();

        // Creates the appropriate user based on argument
        if (args.TryGetValue("-mode", out string mode))
        {
            switch (mode)
            {
                case "server":
                    netManager.StartServer();
                    break;
                case "host":
                    netManager.StartHost();
                    break;
                case "client":

                    netManager.StartClient();
                    break;
            }
        }
    }

    private Dictionary<string, string> GetCommandlineArgs()
    {
        Dictionary<string, string> argDictionary = new Dictionary<string, string>();

        // Pull from the command line
        var args = System.Environment.GetCommandLineArgs();

        // Looks for the argument calls
        for (int i = 0; i < args.Length; ++i)
        {
            var arg = args[i].ToLower();
            // Accumulates the arguments and values for being returned
            if (arg.StartsWith("-"))
            {
                var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
                value = (value?.StartsWith("-") ?? false) ? null : value;

                argDictionary.Add(arg, value);
            }
        }
        return argDictionary;
    }
}
