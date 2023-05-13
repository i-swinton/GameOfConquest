using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCarrier : MonoBehaviour
{

    static DataCarrier instance;

    int playerCount;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        if (instance == this)
        {
            // Don't destroy with level loading
            DontDestroyOnLoad(this);
        }
    }


    public static void AddPlayer()
    {
        instance.playerCount++;
    }


    public static void DestroyCarrier()
    {
        Destroy(instance.gameObject);
    }

}
