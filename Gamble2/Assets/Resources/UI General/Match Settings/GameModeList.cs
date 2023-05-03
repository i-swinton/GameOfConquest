using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeList : MonoBehaviour
{
    [SerializeField] GameMode[] availableGameModes;

    static GameModeList instance;

    public static int Count
    {
        get
        {
            return instance.availableGameModes.Length;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    public static GameMode GetGameMode(int gameModeIndex)
    {
        return instance.availableGameModes[gameModeIndex];
    }

    public static GameMode GetGameMode(string gameModeName)
    {
        for(int i=0; i < Count; ++i)
        {
            if(instance.availableGameModes[i].Name == gameModeName)
            {
                return instance.availableGameModes[i];
            }
        }

        return instance.availableGameModes[0];
    }

    public static int GetGameModeIndex(GameMode mode)
    {
        for(int i=0; i < Count; ++i)
        {
            if(instance.availableGameModes[i] == mode)
            {
                return i;
            }
        }

        return 0;
    }
}
