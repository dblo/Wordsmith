using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MultiplayerLobby : MonoBehaviour {
    private Button wordSeaButton;
    private Slider gameLengthSlider;
    private Slider playercountSlider;
    private Slider seaModeSlider;
    private Slider seaSizeSlider;
    private Slider lineLengthSlider;
    private Text gameLengthLabel;
    private Text playerCountLabel;
    private Text seaModeLabel;
    private Text seaSizeLabel;
    private Text lineLengthLabel;
    private string selectedWordSea = "Default";

    void Start () {
        gameLengthSlider = GameObject.Find("GameLengthSlider").GetComponent<Slider>();
        playercountSlider = GameObject.Find("PlayerCountSlider").GetComponent<Slider>();
        seaModeSlider = GameObject.Find("SeaModeSlider").GetComponent<Slider>();
        seaSizeSlider = GameObject.Find("SeaSizeSlider").GetComponent<Slider>();
        lineLengthSlider = GameObject.Find("LineLengthSlider").GetComponent<Slider>();
        gameLengthLabel = GameObject.Find("GameLengthLabel").GetComponent<Text>();
        playerCountLabel = GameObject.Find("PlayerCountLabel").GetComponent<Text>();
        seaModeLabel = GameObject.Find("SeaModeLabel").GetComponent<Text>();
        seaSizeLabel = GameObject.Find("SeaSizeLabel").GetComponent<Text>();
        lineLengthLabel = GameObject.Find("LineLengthLabel").GetComponent<Text>();
        wordSeaButton = GameObject.Find("LobbyWordSeaListButton").GetComponent<Button>();

        wordSeaButton.onClick.AddListener(() => WordSeaListButtonClicked("Default"));

        var closeLobbyButton = GameObject.Find("LobbyCloseButton").GetComponent<Button>();
        closeLobbyButton.onClick.AddListener(() => Destroy(gameObject));

        gameLengthSlider.onValueChanged.AddListener(
            (value) => gameLengthLabel.text = "Game length: " + value);

        playercountSlider.onValueChanged.AddListener(
             (value) => playerCountLabel.text = "Players: " + value);

        seaModeSlider.onValueChanged.AddListener(
             (value) => seaModeLabel.text = "Sea mode: " + value);

        seaSizeSlider.onValueChanged.AddListener(
             (value) => seaSizeLabel.text = "Sea size: " + value);

        lineLengthSlider.onValueChanged.AddListener(
             (value) => lineLengthLabel.text = "Line length: " + value);

        var defaultPlayerCount = PlayerPrefs.GetInt("DefaultPlayerCount", -1);
        if (defaultPlayerCount >= 0) {
            if (playercountSlider.value == defaultPlayerCount) {
                playerCountLabel.text = "Players: " + defaultPlayerCount;
            }
            playercountSlider.value = defaultPlayerCount;
        } else {
            playerCountLabel.text = "Players: " + playercountSlider.value;
        }

        var defaultGameLength = PlayerPrefs.GetInt("DefaultGameLength", -1);
        if (defaultGameLength >= 0) {
            if (gameLengthSlider.value == defaultGameLength) {
                gameLengthLabel.text = "Game length: " + defaultGameLength;
            }
            gameLengthSlider.value = defaultGameLength;
        } else {
            gameLengthLabel.text = "Game length: " + gameLengthSlider.value;
        }
        SetupLibraries();
    }

    public void WordSeaListButtonClicked(string libraryName) {
        selectedWordSea = libraryName;
    }

    private void SetupLibraries () {
        var libraryNames = PlayerPrefs.GetString(WordSea.PP_LIBRARY_NAMES);
        if (libraryNames == "")
            return;

        foreach (var name in libraryNames.Split(';')) {
            var go = Instantiate(wordSeaButton, wordSeaButton.transform.parent);
            go.GetComponentInChildren<Text>().text = name;
            go.GetComponent<Button>().onClick.AddListener(() => WordSeaListButtonClicked(name));
        }
    }

    public void StartGame () {
        PlayerPrefs.SetInt("DefaultPlayerCount", (int) playercountSlider.value);
        GameManager.ExpectedPlayerCount = (int) playercountSlider.value;

        PlayerPrefs.SetInt("DefaultGameLength", (int) gameLengthSlider.value);
        GameManager.LinesPerGame = (int) gameLengthSlider.value;

        //Todo startgame button invalid until wordsea chosen
        WordSea.currentLibrary = selectedWordSea;

        var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        nm.StartHost();
    }
}
