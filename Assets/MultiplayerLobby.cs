using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MultiplayerLobby : MonoBehaviour {
    private Slider gameSizeSlider;

    void Start () {
        gameSizeSlider = GameObject.Find("GameSizeSlider").GetComponent<Slider>();

        var defaultPlayerCount = PlayerPrefs.GetInt("DefaultPlayerCount", -1);
        if (defaultPlayerCount >= 0) {
            gameSizeSlider.value = defaultPlayerCount;
        }
    }

    public void StartGame() {
        PlayerPrefs.SetInt("DefaultPlayerCount", (int) gameSizeSlider.value);
        GameManager.ExpectedPlayerCount = (int) gameSizeSlider.value;

        var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        nm.StartHost();
    }
}
