using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;

namespace OO {
    public class GameManager : NetworkBehaviour {
        [SerializeField] private WordSea wordSea;
        [SerializeField] private ButtonBar buttonBar;
        [SerializeField] private Text roundDisplay;
        [SerializeField] private Text libraryNameDisplay;
        private List<PlayerConnection> players;

        [SerializeField] private ScorePanel scorePanelPrefab;
        private readonly List<LineLog> lineLogs = new List<LineLog>();
        private ColorMapper colorWordMapper = new ColorMapper();

        public override void OnStartServer () {
            players = new List<PlayerConnection>();
        }

        // Only called on server
        public void PlayerSetupDone (PlayerConnection player) {
            players.Add(player);
            if (!IsGameFull())
                return;

            players.ForEach((p) => p.CmdSynchronizeName());
            var libraryJson = JsonUtility.ToJson(GameData.Library);
            var newWordSea = GenerateNewSea();
            RpcOnAllPlayersJoined(libraryJson, newWordSea, GameData.PlayerCount,
                GameData.GameLength, GameData.PrefferedPicks);
        }

        private static string[] GenerateNewSea () {
            string[] newWordSea = null;
            if (GameData.Library.Type == LibraryType.Free) {
                newWordSea = WordSea.GenerateNewSea(GameData.Library.GetSea(0), GameData.SeaSize);
            } // else leave as null, clients will refer to the library to get the sea each round
            return newWordSea;
        }

        [ClientRpc]
        private void RpcOnAllPlayersJoined (string libraryJson, string[] newWordSea, int playercount,
                                            int gameLength, int lineLength) {
            var library = JsonUtility.FromJson<Library>(libraryJson);
            GameData.NewGame(library, playercount, gameLength, newWordSea.Length, lineLength);

            players = FindSortedPlayers();

            libraryNameDisplay.text = library.Name;
            UpdateRoundDisplay();
            CreateLineLogs();

            buttonBar.GameStarting();
            ChangeSea(newWordSea);
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
                var newWordSea = GenerateNewSea();
                RpcAllPlayersReady(newWordSea);
            }
        }

        [ClientRpc]
        private void RpcAllPlayersReady (string[] newWordSea) {
            RemoveTemporaryWords();

            colorWordMapper.ComputeColors(players);
            colorWordMapper.ComputeScore();
            AddWordsToLineLogs(colorWordMapper.GetColors());
            GameData.NewRound();

            if (GameOver()) {
                buttonBar.OnGameOver();
                ShowGameOverScreen();
            } else {
                UpdateRoundDisplay();
                foreach (var p in players) {
                    p.NewRound();
                }
                buttonBar.NewRound();
                ChangeSea(newWordSea);
            }
        }

        private void ChangeSea (string[] newWordSea) {
            if (newWordSea == null || newWordSea.Length == 0) { // Length check only necessary because UNET doesnt serialize string[] null correctly
                if (GameData.Library.Type == LibraryType.Free)
                    throw new InvalidOperationException("NewWordSea null in Free mode.");
                wordSea.SetNewSea(GameData.Library.GetSea(GameData.CurrentRound));
            } else {
                wordSea.SetNewSea(newWordSea);
            }
        }

        private void UpdateRoundDisplay () {
            int roundNumberOneBased = GameData.CurrentRound + 1;
            roundDisplay.text = roundNumberOneBased + "/" + GameData.GameLength;
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
            var go = Instantiate(scorePanelPrefab, canvas.transform);
            var sp = go.GetComponent<ScorePanel>();

            var maxScore = GameData.Library.GetTotalWordsPicked() * ColorMapper.GREEN_SCORE;
            sp.Setup(colorWordMapper.Score, maxScore);
        }

        private bool AllPlayersReady () {
            foreach (var p in players) {
                if (!p.Ready)
                    return false;
            }
            return true;
        }

        private bool GameOver () {
            return GameData.CurrentRound == GameData.GameLength;
        }

        private bool IsGameFull () {
            return players.Count == GameData.PlayerCount;
        }

        public static void LaunchMainMenu () {
            var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
            nm.StopHost();
            SceneManager.LoadScene("main_menu");
        }
    }
}
