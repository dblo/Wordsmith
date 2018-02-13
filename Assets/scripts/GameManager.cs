using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace OO {
    public class GameManager : NetworkBehaviour {
        public static bool SoundMuted { get; set; }

        [SerializeField] private WordSea wordSea;
        [SerializeField] private ButtonBar buttonBar;
        [SerializeField] private ScorePanel scorePanel;
        [SerializeField] private Text roundDisplay;
        [SerializeField] private Text libraryNameDisplay;
        private readonly List<LineLog> lineLogs = new List<LineLog>();
        private List<PlayerConnection> players;
        private ColorMapper colorWordMapper;
        private int currentRound = 1;

        private void Awake () {
            SoundMuted = Preferences.GetBool(Preferences.SOUND_MUTED);
        }

        public override void OnStartServer () {
            players = new List<PlayerConnection>();
        }

        // Only called on server
        public void PlayerSetupDone (PlayerConnection player) {
            players.Add(player);
            if (!IsGameFull())
                return;

            players.ForEach((p) => p.CmdSynchronizeName());
            var newWordSea = WordSea.GenerateNewSea(GameData.Instance.SelectedLibrary, GameData.Instance.GetSeaSize());

            var libraryJson = JsonUtility.ToJson(GameData.Instance.SelectedLibrary);
            RpcOnAllPlayersJoined(libraryJson, newWordSea, GameData.Instance.GetRoomSize(),
                GameData.Instance.GetGameLength(), GameData.Instance.GetLineLength());
        }

        [ClientRpc]
        private void RpcOnAllPlayersJoined (string libraryJson, string[] newWordSea, int playercount,
                                            int gameLength, int lineLength) {
            var library = JsonUtility.FromJson<Library>(libraryJson);
            GameData.Instance.NewGame(library, playercount, gameLength, newWordSea.Length, lineLength);

            players = FindSortedPlayers();
            colorWordMapper = new ColorMapper(playercount);

            libraryNameDisplay.text = library.name;
            UpdateRoundDisplay();
            CreateLineLogs();
            buttonBar.GameStarting();
            wordSea.UseNewLibrary(library);
            wordSea.SetNewSea(newWordSea);
        }

        private static List<PlayerConnection> FindSortedPlayers () {
            var pcs = new List<PlayerConnection>();
            var gos = GameObject.FindGameObjectsWithTag("Player");
            foreach (var go in gos) {
                var pc = go.GetComponent<PlayerConnection>();
                if (pc.isLocalPlayer) {
                    pcs.Insert(0, pc);
                } else {
                    pcs.Add(pc);
                }
            }
            return pcs;
        }

        private void CreateLineLogs () {
            for (var i = 0; i < players.Count; i++) {
                lineLogs.Add(LineLog.Create((float) i / players.Count,
                    (float) (i + 1) / players.Count, players[i].PlayerName));
            }
        }

        [Command]
        public void CmdPlayerReady () {
            if (!AllPlayersReady())
                return;

            players.ForEach(p => p.CmdSynchronizeWords());
            if (GameOver()) {
                RpcAllPlayersReady(null);
            } else {
                var newWordSea = WordSea.GenerateNewSea(GameData.Instance.SelectedLibrary, GameData.Instance.GetSeaSize());
                RpcAllPlayersReady(newWordSea);
            }
        }

        [ClientRpc]
        private void RpcAllPlayersReady (string[] newWordSea) {
            RemoveTemporaryWords();

            colorWordMapper.ComputeColors(players);
            colorWordMapper.ComputeScore();
            AddWordsToLineLogs(colorWordMapper.GetColors());
            currentRound++;

            if (GameOver()) {
                buttonBar.OnGameOver();
                ShowGameOverScreen();
            } else {
                UpdateRoundDisplay();
                foreach (var p in players) {
                    p.Reset();
                }
                buttonBar.OnNewRound();
                wordSea.SetNewSea(newWordSea);
            }
        }

        private void UpdateRoundDisplay () {
            roundDisplay.text = currentRound + "/" + GameData.Instance.GetGameLength();
        }

        public void AddTemporaryWordsToLineLog (string[] words) {
            for (var i = 0; i < players.Count; i++) {
                if (players[i].isLocalPlayer) {
                    var tempColors = ColorMapper.GetTemporaryWordColors(words.Length);
                    lineLogs[i].AddTemporaryLine(words, tempColors);
                    break;
                }
            }
        }

        private void RemoveTemporaryWords () {
            for (var i = 0; i < players.Count; i++) {
                if (players[i].isLocalPlayer) {
                    lineLogs[i].RemoveTemporaryLine();
                    break;
                }
            }
        }

        private void AddWordsToLineLogs (List<string[]> colors) {
            for (var i = 0; i < players.Count; i++) {
                lineLogs[i].AddLine(players[i].Words, colors[i]);
            }
        }

        private void ShowGameOverScreen () {
            var lineLogsGo = GameObject.Find("LineLogs");
            var rTrans = lineLogsGo.GetComponent<RectTransform>();
            rTrans.anchorMin = new Vector2(0, 0.3f);

            wordSea.gameObject.SetActive(false);

            var canvas = GameObject.Find("Canvas");
            var go = Instantiate(scorePanel, canvas.transform);
            var sp = go.GetComponent<ScorePanel>();
            sp.Setup(colorWordMapper.Scores);
        }

        private bool AllPlayersReady () {
            foreach (var p in players) {
                if (!p.Ready)
                    return false;
            }
            return true;
        }

        private bool GameOver () {
            return currentRound > GameData.Instance.GetGameLength();
        }

        private bool IsGameFull () {
            return players.Count == GameData.Instance.GetRoomSize();
        }

        public static void LaunchMainMenu () {
            var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
            nm.StopHost();
            SceneManager.LoadScene("main_menu");
        }
    }
}
