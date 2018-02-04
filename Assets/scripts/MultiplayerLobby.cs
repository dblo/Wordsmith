using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

namespace OO {
    public class MultiplayerLobby : MonoBehaviour {
        private Button libraryElement;
        private Slider gameLengthSlider;
        private Slider playerCountSlider;
        private Slider seaSizeSlider;
        private Slider lineLengthSlider;
        private Text gameLengthLabel;
        private Text playerCountLabel;
        private Text seaSizeLabel;
        private Text lineLengthLabel;
        private const string DefaultLibraryChoice = "Any";
        private string selectedLibrary = DefaultLibraryChoice;
        private HashSet<string> defaultLibraryNames;

        void Start () {
            gameLengthSlider = GameObject.Find("GameLengthSlider").GetComponent<Slider>();
            playerCountSlider = GameObject.Find("PlayerCountSlider").GetComponent<Slider>();
            seaSizeSlider = GameObject.Find("SeaSizeSlider").GetComponent<Slider>();
            lineLengthSlider = GameObject.Find("LineLengthSlider").GetComponent<Slider>();
            gameLengthLabel = GameObject.Find("GameLengthLabel").GetComponent<Text>();
            playerCountLabel = GameObject.Find("PlayerCountLabel").GetComponent<Text>();
            seaSizeLabel = GameObject.Find("SeaSizeLabel").GetComponent<Text>();
            lineLengthLabel = GameObject.Find("LineLengthLabel").GetComponent<Text>();
            libraryElement = GameObject.Find("LobbyWordSeaListButton").GetComponent<Button>();

            SetupOnClickListeners();
            UsePrefsValuesIfPresent();
            SetupLibraries();
        }

        private void UsePrefsValuesIfPresent () {
            var defaultPlayerCount = PlayerPrefs.GetInt(PreferencesKeys.DefaultPlayerCount, -1);
            if (defaultPlayerCount >= 0) {
                if (GetPlayerCount() == defaultPlayerCount) {
                    playerCountLabel.text = "Players: " + defaultPlayerCount;
                }
                playerCountSlider.value = defaultPlayerCount;
            } else {
                playerCountLabel.text = "Players: " + GetPlayerCount();
            }

            var defaultGameLength = PlayerPrefs.GetInt(PreferencesKeys.DefaultGameLength, -1);
            if (defaultGameLength >= 0) {
                if (GetGameLength() == defaultGameLength) {
                    gameLengthLabel.text = "Game length: " + defaultGameLength;
                }
                gameLengthSlider.value = defaultGameLength;
            } else {
                gameLengthLabel.text = "Game length: " + GetGameLength();
            }
            seaSizeLabel.text = "Sea size: " + GetSeaSize();
            lineLengthLabel.text = "Line length " + GetLineLength();
        }

        private void SetupOnClickListeners () {
            libraryElement.onClick.AddListener(() => WordSeaListButtonClicked("Default"));

            var closeLobbyButton = GameObject.Find("LobbyCloseButton").GetComponent<Button>();
            closeLobbyButton.onClick.AddListener(() => Destroy(gameObject));

            gameLengthSlider.onValueChanged.AddListener(
                (value) => gameLengthLabel.text = "Game length: " + value);

            playerCountSlider.onValueChanged.AddListener(
                 (value) => playerCountLabel.text = "Players: " + value);

            seaSizeSlider.onValueChanged.AddListener(
                 (value) => seaSizeLabel.text = "Sea size: " + value);

            lineLengthSlider.onValueChanged.AddListener(
                 (value) => lineLengthLabel.text = "Line length: " + value);
        }

        private void SetupLibraries () {
            var anyLibGO = Instantiate(libraryElement, libraryElement.transform.parent);
            anyLibGO.GetComponentInChildren<Text>().text = DefaultLibraryChoice;
            anyLibGO.GetComponent<Button>().onClick.AddListener(() => WordSeaListButtonClicked(name));

            var defaultLibNames = PlayerPrefs.GetString(PreferencesKeys.DefaultLibraryNames, "").Split(';');
            if (defaultLibNames.Length == 0)
                throw new InvalidOperationException("No default libraries found in MultiplayerLobby.SetupLibraries()");
            defaultLibraryNames = new HashSet<string>(defaultLibNames);

            foreach (var name in defaultLibNames) {
                var go = Instantiate(libraryElement, libraryElement.transform.parent);
                go.GetComponentInChildren<Text>().text = name;
                go.GetComponent<Button>().onClick.AddListener(() => WordSeaListButtonClicked(name));
            }
            var customLibNames = PlayerPrefs.GetString(PreferencesKeys.CustomLibraryNames, "")
                .Split(GC.LibraryNameDelimiter);

            foreach (var name in customLibNames) {
                var go = Instantiate(libraryElement, libraryElement.transform.parent);
                go.GetComponentInChildren<Text>().text = name;
                go.GetComponent<Button>().onClick.AddListener(() => WordSeaListButtonClicked(name));
            }
        }

        public void StartGame () {
            //Todo startgame button invalid until wordsea chosen
            PlayerPrefs.SetInt(PreferencesKeys.DefaultPlayerCount, GetPlayerCount());
            PlayerPrefs.SetInt(PreferencesKeys.DefaultGameLength, GetGameLength());

            var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
            var mm = GameObject.Find("Canvas").GetComponent<MainMenu>();
            if (mm.InLanMode) {
                ChooseSettingsForAny();
                SetupHostData();
                nm.StartHost();
            } else {
                if (DefaultOrAnyLibraryChosen()) {
                    var roomName = CreateRoomName();
                    NetworkManager.singleton.matchMaker.ListMatches(0, 3, roomName, true, 0, 0, OnMatchList);
                } else {
                    HostMMGame(nm);
                }
            }
        }

        private void ChooseSettingsForAny () {
            if (selectedLibrary == DefaultLibraryChoice) {
                int ix = new System.Random().Next(defaultLibraryNames.Count);
                var iter = defaultLibraryNames.GetEnumerator();
                iter.MoveNext();
                for (int i = 0; i < ix; i++) {
                    iter.MoveNext();
                }
                selectedLibrary = iter.Current;
            }
            //todo handle any settings on the rest
        }

        private string CreateRoomName () {
            string roomName = "";
            if (DefaultLibraryChosen()) {
                roomName = selectedLibrary;
            }
            if (GetPlayerCount() != GC.Any)
                roomName += GC.PlayerCountDelimiter + GetPlayerCount().ToString();
            if (GetGameLength() != GC.Any)
                roomName += GC.GameLengthDelimiter + GetGameLength().ToString();
            if (GetSeaSize() != GC.Any)
                roomName += GC.SeaSizeDelimiter + GetSeaSize().ToString();
            if (GetLineLength() != GC.Any)
                roomName += GC.LineLengthDelimiter + GetLineLength().ToString();
            return roomName;
        }

        private void HostMMGame (NetworkManager nm) {
            ChooseSettingsForAny();
            SetupHostData();

            var roomName = CreateRoomName();
            NetworkManager.singleton.matchMaker.CreateMatch(roomName, (uint) GetPlayerCount(),
                true, "", "", "", 0, 0, nm.OnMatchCreate);
        }

        private void SetupHostData () {
            GameManager.ExpectedPlayerCount = GetPlayerCount();
            GameManager.LinesPerGame = GetGameLength();
            WordSea.currentLibraryName = selectedLibrary;
            ButtonBar.lineLength = GetLineLength();
            WordSea.wordSeaSize = GetSeaSize();
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

        private bool DefaultOrAnyLibraryChosen () {
            return DefaultLibraryChosen() || selectedLibrary == DefaultLibraryChoice;
        }

        private bool DefaultLibraryChosen () {
            return defaultLibraryNames.Contains(selectedLibrary);
        }

        private int GetPlayerCount () {
            return (int) playerCountSlider.value;
        }

        private int GetSeaSize () {
            return (int) seaSizeSlider.value;
        }

        private int GetGameLength () {
            return (int) gameLengthSlider.value;
        }

        private void WordSeaListButtonClicked (string libraryName) {
            selectedLibrary = libraryName;
        }

        private int GetLineLength () {
            return (int) lineLengthSlider.value;
        }
    }
}
