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
        public Transform libraryContent;

        private Slider gameLengthSlider;
        private Slider roomSizeSlider;
        private Slider seaSizeSlider;
        private Slider lineLengthSlider;
        private Text gameLengthLabel;
        private Text roomSizeLabel;
        private Text seaSizeLabel;
        private Text lineLengthLabel;
        private string selectedLibrary = DefaultSelectedLibrary;
        private System.Random rng = new System.Random();

        private const string DefaultSelectedLibrary = "Any";
        private const int DefaultSliderValue = 0;
        public const char PlayerCountDelimiter = '(';
        public const char GameLengthDelimiter = ')';
        public const char SeaSizeDelimiter = '{';
        public const char LineLengthDelimiter = '}';


        void Start () {
            gameLengthSlider = GameObject.Find("GameLengthSlider").GetComponent<Slider>();
            roomSizeSlider = GameObject.Find("PlayerCountSlider").GetComponent<Slider>();
            seaSizeSlider = GameObject.Find("SeaSizeSlider").GetComponent<Slider>();
            lineLengthSlider = GameObject.Find("LineLengthSlider").GetComponent<Slider>();
            gameLengthLabel = GameObject.Find("GameLengthLabel").GetComponent<Text>();
            roomSizeLabel = GameObject.Find("PlayerCountLabel").GetComponent<Text>();
            seaSizeLabel = GameObject.Find("SeaSizeLabel").GetComponent<Text>();
            lineLengthLabel = GameObject.Find("LineLengthLabel").GetComponent<Text>();

            SetupOnClickListeners();
            UsePrefsValuesIfPresent();
            SetupLibraries();
        }

        private void UsePrefsValuesIfPresent () {
            var gameLengthPref = PlayerPrefs.GetInt(Preferences.DefaultGameLength, DefaultSliderValue);
            if (gameLengthPref > DefaultSliderValue)
                GameLength = gameLengthPref;
            OnGameLengthChange(GameLength);

            var playerCountPref = PlayerPrefs.GetInt(Preferences.DefaultPlayerCount, DefaultSliderValue);
            if (playerCountPref > DefaultSliderValue)
                PlayerCount = playerCountPref;
            OnPlayerCountChange(PlayerCount);

            var seaSizePref = PlayerPrefs.GetInt(Preferences.DefaultSeaSize, DefaultSliderValue);
            if (seaSizePref > DefaultSliderValue)
                SeaSize = seaSizePref;
            OnSeaSizeChange(SeaSize);

            var lineLengthPref = PlayerPrefs.GetInt(Preferences.DefaultLineLength, DefaultSliderValue);
            if (lineLengthPref > DefaultSliderValue)
                LineLength = lineLengthPref;
            OnLineLengthChange(LineLength);
        }

        private void SetupOnClickListeners () {
            var closeLobbyButton = GameObject.Find("LobbyCloseButton").GetComponent<Button>();
            closeLobbyButton.onClick.AddListener(() => SceneManager.LoadScene("main_menu"));

            gameLengthSlider.onValueChanged.AddListener(OnGameLengthChange);
            roomSizeSlider.onValueChanged.AddListener(OnPlayerCountChange);
            seaSizeSlider.onValueChanged.AddListener(OnSeaSizeChange);
            lineLengthSlider.onValueChanged.AddListener(OnLineLengthChange);
        }

        private void SetupLibraries () {var go = Instantiate(listElementPrefab, libraryContent);
            go.GetComponentInChildren<Text>().text = DefaultSelectedLibrary;
            go.GetComponent<Button>().onClick.AddListener(() => WordSeaListButtonClicked(DefaultSelectedLibrary));

            foreach (var lib in GameData.Instance.GetLibraries()) {
                go = Instantiate(listElementPrefab, libraryContent);
                go.GetComponentInChildren<Text>().text = lib.name;
                go.GetComponent<Button>().onClick.AddListener(() => WordSeaListButtonClicked(lib.name));
            }
        }

        public void StartGame () {
            PlayerPrefs.SetInt(Preferences.DefaultPlayerCount, PlayerCount);
            PlayerPrefs.SetInt(Preferences.DefaultGameLength, GameLength);
            PlayerPrefs.SetInt(Preferences.DefaultSeaSize, SeaSize);
            PlayerPrefs.SetInt(Preferences.DefaultLineLength, LineLength);

            var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
            if (MainMenu.InLanMode) {
                SetupGameParameters();
                nm.StartHost();
            } else {
                if (!GameData.Instance.GetSelectedLibrary().playerMade || AnyLibrarySelected()) {
                    var roomName = CreateRoomName();
                    NetworkManager.singleton.matchMaker.ListMatches(0, 3, roomName, true, 0, 0, OnMatchList);
                } else {
                    HostMMGame(nm);
                }
            }
        }

        private void SetupGameParameters () {
            // Using locals to prevent sliders momentarily updating as the scene changes
            int roomSize = PlayerCount,
                gameLength = GameLength,
                seaSize = SeaSize,
                lineLength = LineLength;

            if (AnyLibrarySelected()) {
                int randomIndex = rng.Next(GameData.Instance.GetLibraries().Count);
                selectedLibrary = GameData.Instance.GetLibraries()[randomIndex].name;
            }
            if (GameLength == DefaultSliderValue) {
                gameLength = rng.Next(1, (int) gameLengthSlider.maxValue + 1);
            }
            if (PlayerCount == DefaultSliderValue) {
                roomSize = rng.Next(1, (int) roomSizeSlider.maxValue + 1);
            }
            if (SeaSize == DefaultSliderValue && LineLength == DefaultSliderValue) {
                var seaSizeMax = Math.Min(GameData.Instance.GetSelectedLibrary().words.Length,
                    (int) seaSizeSlider.maxValue);
                seaSize = rng.Next(1, seaSizeMax + 1);

                var lineLengthMax = Math.Min((int) lineLengthSlider.maxValue, SeaSize);
                lineLength = rng.Next(1, lineLengthMax + 1);
            } else if (SeaSize == DefaultSliderValue && LineLength != DefaultSliderValue) {
                var seaSizeMax = Math.Min(GameData.Instance.GetSelectedLibrary().words.Length,
                    LineLength);
                seaSize = rng.Next(1, seaSizeMax + 1);
            } else if (SeaSize != DefaultSliderValue && LineLength == DefaultSliderValue) {
                lineLength = rng.Next(1, SeaSize + 1);
            } else if (SeaSize != DefaultSliderValue && LineLength != DefaultSliderValue) {
            }
            // todo Make it so a player cannot choose values so belows are needed
            if (seaSize > GameData.Instance.GetSelectedLibrary().words.Length)
                seaSize = GameData.Instance.GetSelectedLibrary().words.Length;
            if (lineLength > seaSize)
                lineLength = seaSize;

            GameData.Instance.NewGame(selectedLibrary, roomSize, gameLength, seaSize, lineLength);
        }

        private string CreateRoomName () {
            string roomName = "";
            if (!GameData.Instance.GetSelectedLibrary().playerMade) {
                roomName = selectedLibrary;
            }
            if (PlayerCount != DefaultSliderValue)
                roomName += PlayerCountDelimiter + PlayerCount.ToString();
            if (GameLength != DefaultSliderValue)
                roomName += GameLengthDelimiter + GameLength.ToString();
            if (SeaSize != DefaultSliderValue)
                roomName += SeaSizeDelimiter + SeaSize.ToString();
            if (LineLength != DefaultSliderValue)
                roomName += LineLengthDelimiter + LineLength.ToString();
            return roomName;
        }

        private void HostMMGame (NetworkManager nm) {
            SetupGameParameters();

            var roomName = CreateRoomName();
            NetworkManager.singleton.matchMaker.CreateMatch(roomName, (uint) PlayerCount,
                true, "", "", "", 0, 0, nm.OnMatchCreate);
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

        private bool AnyLibrarySelected () {
            return selectedLibrary.Equals(DefaultSelectedLibrary);
        }

        private int GameLength {
            get { return (int) gameLengthSlider.value; }
            set { gameLengthSlider.value = value; }
        }

        private int PlayerCount {
            get { return (int) roomSizeSlider.value; }
            set { roomSizeSlider.value = value; }
        }

        private int SeaSize {
            get { return (int) seaSizeSlider.value; }
            set { seaSizeSlider.value = value; }
        }

        private int LineLength {
            get { return (int) lineLengthSlider.value; }
            set { lineLengthSlider.value = value; }
        }

        private void WordSeaListButtonClicked (string library) {
            selectedLibrary = library;
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
                roomSizeLabel.text = "Players: Any";
            } else {
                roomSizeLabel.text = "Players: " + value;
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
