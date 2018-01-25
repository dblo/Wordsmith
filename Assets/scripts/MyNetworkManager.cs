using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager
{
    public int devConnectionCount = 0; //Development helper
    private static int playerCount = 0;

    public static int PlayerCount
    {
        get { return playerCount; }
        set { playerCount = value; }
    }

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (LaunchingSceneFromEditor())
            playerCount = devConnectionCount;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "main")
        {
            if (PlayerCount == 1)
            {
                StartHost();
            }
        }
    }

    private static bool LaunchingSceneFromEditor()
    {
        return playerCount == 0;
    }
}
