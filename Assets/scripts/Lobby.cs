using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OO {
    public class Lobby : MonoBehaviour {
        public Button listElementPrefab;

        private Slider gameLengthSlider;
        private Slider playerCountSlider;
        private Slider seaSizeSlider;
        private Slider lineLengthSlider;
        private Text gameLengthLabel;
        private Text playerCountLabel;
        private Text seaSizeLabel;
        private Text lineLengthLabel;
        private string selectedLibrary = DefaultSelectedLibrary;
        private List<string> defaultLibraryNames;
        private System.Random rng = new System.Random();
        private int customLibraryCount = 0;

        private const string DefaultSelectedLibrary = "Any";
        private const int DefaultSliderValue = 0;

        void Start () {
            gameLengthSlider = GameObject.Find("GameLengthSlider").GetComponent<Slider>();
            playerCountSlider = GameObject.Find("PlayerCountSlider").GetComponent<Slider>();
            seaSizeSlider = GameObject.Find("SeaSizeSlider").GetComponent<Slider>();
            lineLengthSlider = GameObject.Find("LineLengthSlider").GetComponent<Slider>();
            gameLengthLabel = GameObject.Find("GameLengthLabel").GetComponent<Text>();
            playerCountLabel = GameObject.Find("PlayerCountLabel").GetComponent<Text>();
            seaSizeLabel = GameObject.Find("SeaSizeLabel").GetComponent<Text>();
            lineLengthLabel = GameObject.Find("LineLengthLabel").GetComponent<Text>();

            SetupOnClickListeners();
            UsePrefsValuesIfPresent();
            SetupLibraries();
        }

        private void UsePrefsValuesIfPresent () {
            var gameLengthPref = PlayerPrefs.GetInt(PreferencesKeys.DefaultGameLength, DefaultSliderValue);
            if (gameLengthPref > DefaultSliderValue)
                GameLength = gameLengthPref;
            OnGameLengthChange(GameLength);

            var playerCountPref = PlayerPrefs.GetInt(PreferencesKeys.DefaultPlayerCount, DefaultSliderValue);
            if (playerCountPref > DefaultSliderValue)
                PlayerCount = playerCountPref;
            OnPlayerCountChange(PlayerCount);

            var seaSizePref = PlayerPrefs.GetInt(PreferencesKeys.DefaultSeaSize, DefaultSliderValue);
            if (seaSizePref > DefaultSliderValue)
                SeaSize = seaSizePref;
            OnSeaSizeChange(SeaSize);

            var lineLengthPref = PlayerPrefs.GetInt(PreferencesKeys.DefaultLineLength, DefaultSliderValue);
            if (lineLengthPref > DefaultSliderValue)
                LineLength = lineLengthPref;
            OnLineLengthChange(LineLength);
        }

        private void SetupOnClickListeners () {
            var anyLibraryListElement = GameObject.Find("LobbyWordSeaListButton").GetComponent<Button>();
            anyLibraryListElement.onClick.AddListener(() => WordSeaListButtonClicked("Default"));

            var closeLobbyButton = GameObject.Find("LobbyCloseButton").GetComponent<Button>();
            closeLobbyButton.onClick.AddListener(() => SceneManager.LoadScene("main_menu"));

            gameLengthSlider.onValueChanged.AddListener(OnGameLengthChange);
            playerCountSlider.onValueChanged.AddListener(OnPlayerCountChange);
            seaSizeSlider.onValueChanged.AddListener(OnSeaSizeChange);
            lineLengthSlider.onValueChanged.AddListener(OnLineLengthChange);
        }

        private void SetupLibraries () {
            defaultLibraryNames = new List<string>(PlayerPrefs.GetString(PreferencesKeys.DefaultLibraryNames, "").Split(';'));
            if (defaultLibraryNames.Count == 0)
                throw new InvalidOperationException("No default libraries found in MultiplayerLobby.SetupLibraries()");

            var parent = GameObject.Find("LobbyWordSeaListButton").transform.parent;
            foreach (var name in defaultLibraryNames) {
                var go = Instantiate(listElementPrefab, parent);
                go.GetComponentInChildren<Text>().text = name;
                go.GetComponent<Button>().onClick.AddListener(() => WordSeaListButtonClicked(name));
            }
            var customLibString = PlayerPrefs.GetString(PreferencesKeys.CustomLibraryNames, "");
            if (customLibString != "") {
                var customLibNames = customLibString.Split(GC.LibraryNameDelimiter);
                foreach (var name in customLibNames) {
                    if (name == "")
                        continue; // todo remove

                    customLibraryCount++;
                    var go = Instantiate(listElementPrefab, parent);
                    go.GetComponentInChildren<Text>().text = name;
                    go.GetComponent<Button>().onClick.AddListener(() => WordSeaListButtonClicked(name));
                }
            }
        }

        public void StartGame () {
            PlayerPrefs.SetInt(PreferencesKeys.DefaultPlayerCount, PlayerCount);
            PlayerPrefs.SetInt(PreferencesKeys.DefaultGameLength, GameLength);
            PlayerPrefs.SetInt(PreferencesKeys.DefaultSeaSize, SeaSize);
            PlayerPrefs.SetInt(PreferencesKeys.DefaultLineLength, LineLength);

            var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
            if (MainMenu.InLanMode) {
                ChooseSettingsForAny();
                SetupHostData();
                nm.StartHost();
            } else {
                if (DefaultLibrarySelected() || AnyLibrarySelected()) {
                    var roomName = CreateRoomName();
                    NetworkManager.singleton.matchMaker.ListMatches(0, 3, roomName, true, 0, 0, OnMatchList);
                } else {
                    HostMMGame(nm);
                }
            }
        }

        private void ChooseSettingsForAny () {
            if (AnyLibrarySelected()) {
                int randomIndex = rng.Next(defaultLibraryNames.Count + customLibraryCount);
                string libName;
                if (randomIndex < defaultLibraryNames.Count) {
                    libName = defaultLibraryNames[randomIndex];
                } else {
                    var customLibString = PlayerPrefs.GetString(PreferencesKeys.CustomLibraryNames, "");
                    var customLibNames = customLibString.Split(GC.LibraryNameDelimiter);
                    libName = customLibNames[customLibraryCount - randomIndex];
                }
                selectedLibrary = libName;
            }
            if (GameLength == DefaultSliderValue)
                GameLength = rng.Next(1, (int) gameLengthSlider.maxValue + 1);

            if (PlayerCount == DefaultSliderValue)
                PlayerCount = rng.Next(1, (int) playerCountSlider.maxValue + 1);

            if (SeaSize == DefaultSliderValue) {
                //todo make sure seasize <= length of selected library size
                SeaSize = rng.Next(1, (int) seaSizeSlider.maxValue + 1);
            }
            if (LineLength == DefaultSliderValue) {
                var lineLengthMax = Math.Min((int) lineLengthSlider.maxValue + 1, SeaSize);
                LineLength = rng.Next(1, lineLengthMax);
            }
        }

        private string CreateRoomName () {
            string roomName = "";
            if (DefaultLibrarySelected()) {
                roomName = selectedLibrary;
            }
            if (PlayerCount != DefaultSliderValue)
                roomName += GC.PlayerCountDelimiter + PlayerCount.ToString();
            if (GameLength != DefaultSliderValue)
                roomName += GC.GameLengthDelimiter + GameLength.ToString();
            if (SeaSize != DefaultSliderValue)
                roomName += GC.SeaSizeDelimiter + SeaSize.ToString();
            if (LineLength != DefaultSliderValue)
                roomName += GC.LineLengthDelimiter + LineLength.ToString();
            return roomName;
        }

        private void HostMMGame (NetworkManager nm) {
            ChooseSettingsForAny();
            SetupHostData();

            var roomName = CreateRoomName();
            NetworkManager.singleton.matchMaker.CreateMatch(roomName, (uint) PlayerCount,
                true, "", "", "", 0, 0, nm.OnMatchCreate);
        }

        private void SetupHostData () {
            GameManager.ExpectedPlayerCount = PlayerCount;
            GameManager.LinesPerGame = GameLength;
            WordSea.currentLibraryName = selectedLibrary;
            ButtonBar.lineLength = LineLength;
            WordSea.wordSeaSize = SeaSize;
        }

        private void OnMatchList (bool success, string extendedInfo, List<MatchInfoSnapshot> responseData) {
            if (!success)
                return; // todo what?

            var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
            if (responseData.Count == 0) {
                HostMMGame(nm);
            } else {
                var match = responseData[0];
                NetworkManager.singleton.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, nm.OnMatchJoined);
            }
        }

        private bool DefaultLibrarySelected () {
            return defaultLibraryNames.Contains(selectedLibrary);
        }

        private bool AnyLibrarySelected () {
            return selectedLibrary == DefaultSelectedLibrary;
        }

        private int GameLength {
            get { return (int) gameLengthSlider.value; }
            set { gameLengthSlider.value = value; }
        }

        private int PlayerCount {
            get { return (int) playerCountSlider.value; }
            set { playerCountSlider.value = value; }
        }

        private int SeaSize {
            get { return (int) seaSizeSlider.value; }
            set { seaSizeSlider.value = value; }
        }

        private int LineLength {
            get { return (int) lineLengthSlider.value; }
            set { lineLengthSlider.value = value; }
        }

        private void WordSeaListButtonClicked (string libraryName) {
            selectedLibrary = libraryName;
        }

        private void OnGameLengthChange (float value) {
            if (value == DefaultSliderValue) {
                gameLengthLabel.text = "Game length: Any";
            } else {
                gameLengthLabel.text = "Game length: " + value;
            }
        }

        private void OnPlayerCountChange (float value) {
            if (value == DefaultSliderValue) {
                playerCountLabel.text = "Players: Any";
            } else {
                playerCountLabel.text = "Players: " + value;
            }
        }

        private void OnSeaSizeChange (float value) {
            if (value == DefaultSliderValue) {
                seaSizeLabel.text = "Sea size: Any";
            } else {
                seaSizeLabel.text = "Sea size: " + value;
            }
        }

        private void OnLineLengthChange (float value) {
            if (value == DefaultSliderValue) {
                lineLengthLabel.text = "Line length: Any";
            } else {
                lineLengthLabel.text = "Line length: " + value;
            }
        }
    }
}
