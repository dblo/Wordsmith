using UnityEngine;
using UnityEngine.Networking;

public class MainMenu : MonoBehaviour {
    public MultiplayerLobby lobbyPrefab;
    public SeaMaker seaMakerPrefab;

    private void Awake () {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public void LaunchLobby () {
        var canvas = GameObject.Find("Canvas");
        Instantiate(lobbyPrefab, canvas.transform);
    }

    public void LaunchSeaMaker() {
        var canvas = GameObject.Find("Canvas");
        Instantiate(seaMakerPrefab, canvas.transform);
    }

    public void JoinAnyGame () {
        GameManager.ExpectedPlayerCount = 2;
        var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        nm.StartClient();
    }

    public void LaunchSettings() {
        var parent = GameObject.Find("Canvas").transform;
        Instantiate(Resources.Load("SettingsDialog"), parent);
    }
}
