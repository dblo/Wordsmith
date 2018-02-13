using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OO {
    public class Lobby : MonoBehaviour {
        [SerializeField] private LibraryList libraryList;

        private Slider gameLengthSlider;
        private Slider roomSizeSlider;
        private Slider seaSizeSlider;
        private Slider lineLengthSlider;
        private Text gameLengthLabel;
        private Text roomSizeLabel;
        private Text seaSizeLabel;
        private Text lineLengthLabel;
        private System.Random rng = new System.Random();

        private const string ANY_LIBRARY = "Any";
        private const int DEFAULT_SLIDER_VALUE = 0;
        private const int SEA_SIZE_DEFAULT_MAX = 12;
        private const int LINE_LENGTH_DEFAULT_MAX = 4;
        private const char PLAYER_COUNT_DELIMITER = '(';
        private const char GAME_LENGTH_DELIMITER = ')';
        private const char SEA_SIZE_DELIMITER = '{';
        private const char LINE_LENGTH_DELIMITER = '}';

        private void Awake () {
            gameLengthSlider = GameObject.Find("GameLengthSlider").GetComponent<Slider>();
            roomSizeSlider = GameObject.Find("PlayerCountSlider").GetComponent<Slider>();
            seaSizeSlider = GameObject.Find("SeaSizeSlider").GetComponent<Slider>();
            lineLengthSlider = GameObject.Find("LineLengthSlider").GetComponent<Slider>();
            gameLengthLabel = GameObject.Find("GameLengthLabel").GetComponent<Text>();
            roomSizeLabel = GameObject.Find("PlayerCountLabel").GetComponent<Text>();
            seaSizeLabel = GameObject.Find("SeaSizeLabel").GetComponent<Text>();
            lineLengthLabel = GameObject.Find("LineLengthLabel").GetComponent<Text>();

            seaSizeSlider.maxValue = SEA_SIZE_DEFAULT_MAX;
            lineLengthSlider.maxValue = LINE_LENGTH_DEFAULT_MAX;

            SetupOnClickListeners();
            UsePrefsValuesIfPresent();

            if (GameData.Instance.LibraryCount == 0) {
                var playButton = transform.Find("PlayButton").GetComponent<Button>();
                playButton.interactable = false;

                var anyLibraryButton = GameObject.Find("AnyLibraryButton");
                anyLibraryButton.gameObject.SetActive(false);
            }
        }

        private void UsePrefsValuesIfPresent () {
            var gameLengthPref = PlayerPrefs.GetInt(Preferences.DEFAULT_GAME_LENGTH, DEFAULT_SLIDER_VALUE);
            if (gameLengthPref > DEFAULT_SLIDER_VALUE)
                GameLength = gameLengthPref;
            OnGameLengthChange(GameLength);

            var playerCountPref = PlayerPrefs.GetInt(Preferences.DEFAULT_PLAYER_COUNT, DEFAULT_SLIDER_VALUE);
            if (playerCountPref > DEFAULT_SLIDER_VALUE)
                PlayerCount = playerCountPref;
            OnPlayerCountChange(PlayerCount);

            var seaSizePref = PlayerPrefs.GetInt(Preferences.DEFAULT_SEA_SIZE, DEFAULT_SLIDER_VALUE);
            if (seaSizePref > DEFAULT_SLIDER_VALUE)
                SeaSize = seaSizePref;
            OnSeaSizeChange(SeaSize);

            var lineLengthPref = PlayerPrefs.GetInt(Preferences.DEFAULT_LINE_LENGTH, DEFAULT_SLIDER_VALUE);
            if (lineLengthPref > DEFAULT_SLIDER_VALUE)
                LineLength = lineLengthPref;
            OnLineLengthChange(LineLength);
        }

        private void SetupOnClickListeners () {
            var closeLobbyButton = GameObject.Find("CloseButton").GetComponent<Button>();
            closeLobbyButton.onClick.AddListener(() => SceneManager.LoadScene("main_menu"));

            gameLengthSlider.onValueChanged.AddListener(OnGameLengthChange);
            roomSizeSlider.onValueChanged.AddListener(OnPlayerCountChange);
            seaSizeSlider.onValueChanged.AddListener(OnSeaSizeChange);
            lineLengthSlider.onValueChanged.AddListener(OnLineLengthChange);

            libraryList.AddSelectedListener(OnLibrarySelectionChange);
        }

        private void OnLibrarySelectionChange () {
            var selectedLibrary = libraryList.GetSelectedLibrary().Library;
            if (selectedLibrary == null)
                return;

            seaSizeSlider.maxValue = Math.Min(SEA_SIZE_DEFAULT_MAX, selectedLibrary.Words.Length);
        }

        public void StartGame () {
            Debug.Assert(GameData.Instance.LibraryCount > 0);

            PlayerPrefs.SetInt(Preferences.DEFAULT_PLAYER_COUNT, PlayerCount);
            PlayerPrefs.SetInt(Preferences.DEFAULT_GAME_LENGTH, GameLength);
            PlayerPrefs.SetInt(Preferences.DEFAULT_SEA_SIZE, SeaSize);
            PlayerPrefs.SetInt(Preferences.DEFAULT_LINE_LENGTH, LineLength);

            var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
            if (MainMenu.InLanMode) {
                SetupGameParameters();
                nm.StartHost();
            } else {
                //if (AnyLibrarySelected() || !GameData.Instance.GetSelectedLibrary().playerMade) {
                //    var roomName = CreateRoomName();
                //    NetworkManager.singleton.matchMaker.ListMatches(0, 3, roomName, true, 0, 0, OnMatchList);
                //} else {
                HostMmGame(nm);
                //}
            }
        }

        private void SetupGameParameters () {
            // Using locals to prevent sliders momentarily updating as the scene changes
            int roomSize = PlayerCount,
                gameLength = GameLength,
                seaSize = SeaSize,
                lineLength = LineLength;

            var selectedLibrary = libraryList.GetSelectedLibrary().Library;
            if (selectedLibrary == null) {
                Debug.Assert(GameData.Instance.LibraryCount > 0);
                int randomIndex = rng.Next(GameData.Instance.LibraryCount);
                selectedLibrary = GameData.Instance.GetLibrary(randomIndex);
            }
            if (GameLength == DEFAULT_SLIDER_VALUE) {
                gameLength = rng.Next(1, (int) gameLengthSlider.maxValue + 1);
            }
            if (PlayerCount == DEFAULT_SLIDER_VALUE) {
                roomSize = rng.Next(1, (int) roomSizeSlider.maxValue + 1);
            }
            if (SeaSize == DEFAULT_SLIDER_VALUE && LineLength == DEFAULT_SLIDER_VALUE) {
                var seaSizeMax = Math.Min(selectedLibrary.Words.Length,
                    (int) seaSizeSlider.maxValue);
                seaSize = rng.Next(1, seaSizeMax + 1);

                var lineLengthMax = Math.Min((int) lineLengthSlider.maxValue, seaSize);
                lineLength = rng.Next(1, lineLengthMax + 1);
            } else if (SeaSize == DEFAULT_SLIDER_VALUE && LineLength != DEFAULT_SLIDER_VALUE) {
                var seaSizeMax = Math.Min(selectedLibrary.Words.Length,
                    LineLength);
                seaSize = rng.Next(1, seaSizeMax + 1);
            } else if (SeaSize != DEFAULT_SLIDER_VALUE && LineLength == DEFAULT_SLIDER_VALUE) {
                lineLength = rng.Next(1, SeaSize + 1);
            } else if (SeaSize != DEFAULT_SLIDER_VALUE && LineLength != DEFAULT_SLIDER_VALUE) {
            }
            Debug.Assert(seaSize <= selectedLibrary.Words.Length);
            Debug.Assert(lineLength <= seaSize);
            GameData.Instance.NewGame(selectedLibrary, roomSize, gameLength, seaSize, lineLength);
        }

        private string CreateRoomName () {
            string roomName = "";
            var selectedLibrary = GameData.Instance.SelectedLibrary;
            if (!selectedLibrary.PlayerMade) {
                roomName = selectedLibrary.Name;
            } // else leave as empty string

            if (PlayerCount != DEFAULT_SLIDER_VALUE)
                roomName += PLAYER_COUNT_DELIMITER + PlayerCount.ToString();
            if (GameLength != DEFAULT_SLIDER_VALUE)
                roomName += GAME_LENGTH_DELIMITER + GameLength.ToString();
            if (SeaSize != DEFAULT_SLIDER_VALUE)
                roomName += SEA_SIZE_DELIMITER + SeaSize.ToString();
            if (LineLength != DEFAULT_SLIDER_VALUE)
                roomName += LINE_LENGTH_DELIMITER + LineLength.ToString();
            return roomName;
        }

        private void HostMmGame (NetworkManager nm) {
            SetupGameParameters();

            var roomName = CreateRoomName();
            NetworkManager.singleton.matchMaker.CreateMatch(roomName, (uint) PlayerCount,
                true, "", "", "", 0, 0, nm.OnMatchCreate);
        }

        //private void OnMatchList (bool success, string extendedInfo, List<MatchInfoSnapshot> responseData) {
        //    if (!success)
        //        return;

        //    var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        //    if (responseData.Count == 0) {
        //        HostMMGame(nm);
        //    } else {
        //        var match = responseData[0];
        //        NetworkManager.singleton.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, nm.OnMatchJoined);
        //    }
        //}

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

        private void OnGameLengthChange (float value) {
            if ((int) value == DEFAULT_SLIDER_VALUE) {
                gameLengthLabel.text = "Game length: Any";
            } else {
                gameLengthLabel.text = "Game length: " + value;
            }
        }

        private void OnPlayerCountChange (float value) {
            if ((int) value == DEFAULT_SLIDER_VALUE) {
                roomSizeLabel.text = "Players: Any";
            } else {
                roomSizeLabel.text = "Players: " + value;
            }
        }

        private void OnSeaSizeChange (float value) {

            if ((int) value == DEFAULT_SLIDER_VALUE) {
                seaSizeLabel.text = "Sea size: Any";
            } else {
                seaSizeLabel.text = "Sea size: " + value;
                lineLengthSlider.maxValue = Math.Min(LINE_LENGTH_DEFAULT_MAX, value);
            }
        }

        private void OnLineLengthChange (float value) {
            if ((int) value == DEFAULT_SLIDER_VALUE) {
                lineLengthLabel.text = "Line length: Any";
            } else {
                lineLengthLabel.text = "Line length: " + value;
            }
        }
    }
}
