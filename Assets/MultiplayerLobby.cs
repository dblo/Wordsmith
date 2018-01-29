using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MultiplayerLobby : MonoBehaviour {
    private Slider gameSizeSlider;

    void Start () {
        gameSizeSlider = GameObject.Find("GameSizeSlider").GetComponent<Slider>();
        gameSizeSlider.onValueChanged.AddListener(delegate { OnGameSizeSliderChange(); });
    }

    public void OnGameSizeSliderChange () {
        GameManager.ExpectedPlayerCount = (int) gameSizeSlider.value;
    }

    public void StartGame() {
        var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        nm.StartHost();
    }
}
