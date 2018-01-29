using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MultiplayerLobby : MonoBehaviour {
    private Slider gameSizeSlider;
    private Slider gameLengthSlider;

    void Start () {
        gameSizeSlider = GameObject.Find("PlayerCountSlider").GetComponent<Slider>();
        gameLengthSlider = GameObject.Find("GameLengthSlider").GetComponent<Slider>();

        var defaultPlayerCount = PlayerPrefs.GetInt("DefaultPlayerCount", -1);
        if (defaultPlayerCount >= 0) {
            gameSizeSlider.value = defaultPlayerCount;
        }

        var defaultGameLength = PlayerPrefs.GetInt("DefaultGameLength", -1);
        if (defaultGameLength >= 0) {
            gameLengthSlider.value = defaultGameLength;
        }
    }

    public void StartGame() {
        PlayerPrefs.SetInt("DefaultPlayerCount", (int) gameSizeSlider.value);
        GameManager.ExpectedPlayerCount = (int) gameSizeSlider.value;

        PlayerPrefs.SetInt("DefaultGameLength", (int) gameLengthSlider.value);
        GameManager.LinesPerGame = (int) gameLengthSlider.value;

        var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        nm.StartHost();
    }
}
