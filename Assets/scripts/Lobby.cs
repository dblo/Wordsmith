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
        private System.Random rng = new System.Random();

        private const string DefaultSelectedLibrary = "Any";
        private const int DefaultSliderValue = 0;
        public const char PlayerCountDelimiter = '(';
        public const char GameLengthDelimiter = ')';
        public const char SeaSizeDelimiter = '{';
        public const char LineLengthDelimiter = '}';


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
            UseLastUsedSettings();
            SetupLibraries();
        }

        private void UseLastUsedSettings () {
            GameLength = GameData.Instance.GetGameLength(); ;
            OnGameLengthChange(GameLength);

            PlayerCount = GameData.Instance.GetRoomSize();
            OnPlayerCountChange(PlayerCount);

            SeaSize = GameData.Instance.GetSeaSize();
            OnSeaSizeChange(SeaSize);

            LineLength = GameData.Instance.GetLineLength();
            OnLineLengthChange(LineLength);
        }

        private void SetupOnClickListeners () {
            var anyLibraryListElement = GameObject.Find("LobbyWordSeaListButton").GetComponent<Button>();
            anyLibraryListElement.GetComponentInChildren<Text>().text = DefaultSelectedLibrary;
            anyLibraryListElement.onClick.AddListener(() => WordSeaListButtonClicked(null));

            var closeLobbyButton = GameObject.Find("LobbyCloseButton").GetComponent<Button>();
            closeLobbyButton.onClick.AddListener(() => SceneManager.LoadScene("main_menu"));

            gameLengthSlider.onValueChanged.AddListener(OnGameLengthChange);
            playerCountSlider.onValueChanged.AddListener(OnPlayerCountChange);
            seaSizeSlider.onValueChanged.AddListener(OnSeaSizeChange);
            lineLengthSlider.onValueChanged.AddListener(OnLineLengthChange);
        }

        private void SetupLibraries () {
            var parent = GameObject.Find("LobbyWordSeaListButton").transform.parent;

            foreach (var lib in GameData.Instance.GetLibraries()) {
                var go = Instantiate(listElementPrefab, parent);
                go.GetComponentInChildren<Text>().text = lib.name;
                go.GetComponent<Button>().onClick.AddListener(() => WordSeaListButtonClicked(lib.name));
            }
        }

        public void StartGame () {
            var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
            if (MainMenu.InLanMode) {
                SetupGameParameters();

                //todo make sure seasize <= length of selected library size
                if (SeaSize < LineLength)
                    LineLength = SeaSize;

                SetupHostData();
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
            if (AnyLibrarySelected()) {
                int randomIndex = rng.Next(GameData.Instance.GetLibraries().Count);
                selectedLibrary = GameData.Instance.GetLibraries()[randomIndex].name;
            }
            if (GameLength == DefaultSliderValue) {
                GameLength = rng.Next(1, (int) gameLengthSlider.maxValue + 1);
            }
            if (PlayerCount == DefaultSliderValue) {
                PlayerCount = rng.Next(1, (int) playerCountSlider.maxValue + 1);
            }
            if (SeaSize == DefaultSliderValue && LineLength == DefaultSliderValue) {
                var seaSizeMax = Math.Min(GameData.Instance.GetSelectedLibrary().words.Length,
                    (int) seaSizeSlider.maxValue);
                SeaSize = rng.Next(1, seaSizeMax + 1);

                var lineLengthMax = Math.Min((int) lineLengthSlider.maxValue, SeaSize);
                LineLength = rng.Next(1, lineLengthMax + 1);
            } else if (SeaSize == DefaultSliderValue && LineLength != DefaultSliderValue) {
                var seaSizeMax = Math.Min(GameData.Instance.GetSelectedLibrary().words.Length,
                    LineLength);
                SeaSize = rng.Next(1, seaSizeMax + 1);
            } else if (SeaSize != DefaultSliderValue && LineLength == DefaultSliderValue) {
                LineLength = rng.Next(1, SeaSize + 1);
            } else if (SeaSize != DefaultSliderValue && LineLength != DefaultSliderValue) {
            }
            // todo Make it so a player cannot choose values so belows are needed
            if (SeaSize > GameData.Instance.GetSelectedLibrary().words.Length)
                SeaSize = GameData.Instance.GetSelectedLibrary().words.Length;
            if (LineLength > SeaSize)
                LineLength = SeaSize;
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
            SetupHostData();

            var roomName = CreateRoomName();
            NetworkManager.singleton.matchMaker.CreateMatch(roomName, (uint) PlayerCount,
                true, "", "", "", 0, 0, nm.OnMatchCreate);
        }

        private void SetupHostData () {
            GameData.Instance.NewGame(selectedLibrary, PlayerCount, GameLength, SeaSize, LineLength);
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
