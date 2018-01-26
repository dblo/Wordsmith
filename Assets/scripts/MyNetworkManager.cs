using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager {
    public int devConnectionCount = 0; //Development helper
    private static int expectedPlayerCount = 0;

    public static int ExpectedPlayerCount {
        get { return expectedPlayerCount; }
        set { expectedPlayerCount = value; }
    }

    private void Awake () {
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (LaunchingSceneFromEditor())
            expectedPlayerCount = devConnectionCount;
    }

    private void OnSceneLoaded (Scene scene, LoadSceneMode mode) {
        if (scene.name == "main") {
            if (ExpectedPlayerCount == 1) {
                StartHost();
            }
        }
    }

    private static bool LaunchingSceneFromEditor () {
        return expectedPlayerCount == 0;
    }
}
