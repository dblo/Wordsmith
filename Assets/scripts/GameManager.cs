using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

namespace OO {
    public class GameManager : NetworkBehaviour {
        public WordSea wordSea;
        public ButtonBar buttonBar;
        public ScorePanel scorePanelPrefab;
        public bool SoundMuted { get; private set; }

        private List<LineLog> lineLogs = new List<LineLog>();
        private List<PlayerConnection> players;
        private ColorMapper colorWordMapper;
        private Text roundDisplay;
        private Text libraryNameDisplay;
        private int currentRound = 1;

        private void Awake () {
            SoundMuted = Preferences.GetBool(Preferences.SoundMuted);
            roundDisplay = GameObject.Find("RoundDisplay").GetComponent<Text>();
            libraryNameDisplay = GameObject.Find("LibraryNameDisplay").GetComponent<Text>();
        }

        public override void OnStartServer () {
            players = new List<PlayerConnection>();
        }

        internal int[] GetScores () {
            return colorWordMapper.GetScores();
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

        public void SetSoundMuted (bool value) {
            SoundMuted = value;
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

        private List<PlayerConnection> FindSortedPlayers () {
            var pcs = new List<PlayerConnection>();
            var gos = GameObject.FindGameObjectsWithTag("Player");
            foreach (var go in gos) {
                var pc = go.GetComponent<PlayerConnection>();
                if (pc.isLocalPlayer)
                    pcs.Insert(0, pc);
                else
                    pcs.Add(pc);
            }
            return pcs;
        }

        private void CreateLineLogs () {
            for (int i = 0; i < players.Count; i++) {
                lineLogs.Add(LineLog.Create((float) i / players.Count,
                    (float) (i + 1) / players.Count, players[i].PlayerName));
            }
        }

        [Command]
        public void CmdPlayerReady () {
            if (!AllPlayersReady())
                return;

            players.ForEach((p) => p.CmdSynchronizeWords());
            var newWordSea = GameOver() ? null : wordSea.GenerateNewSea();
            RpcAllPlayersReady(newWordSea);
        }

        [ClientRpc]
        void RpcAllPlayersReady (string[] newWordSea) {
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
            for (int i = 0; i < players.Count; i++) {
                if (players[i].isLocalPlayer) {
                    var tempColors = ColorMapper.GetTemporaryWordColors(words.Length);
                    lineLogs[i].AddTemporaryLine(words, tempColors);
                    break;
                }
            }
        }

        private void RemoveTemporaryWords () {
            for (int i = 0; i < players.Count; i++) {
                if (players[i].isLocalPlayer) {
                    lineLogs[i].RemoveTemporaryLine();
                    break;
                }
            }
        }

        private void AddWordsToLineLogs (List<string[]> colors) {
            for (int i = 0; i < players.Count; i++) {
                lineLogs[i].AddLine(players[i].Words, colors[i]);
            }
        }

        private void ShowGameOverScreen () {
            var lineLogsGO = GameObject.Find("LineLogs");
            var rTrans = lineLogsGO.GetComponent<RectTransform>();
            rTrans.anchorMin = new Vector2(0, 0.3f);

            wordSea.gameObject.SetActive(false);

            var canvas = GameObject.Find("Canvas");
            Instantiate(scorePanelPrefab, canvas.transform);
        }

        private bool AllPlayersReady () {
            foreach (var p in players) {
                if (!p.Ready)
                    return false;
            }
            return true;
        }

        public void SetUIButtonsInteractable (bool value) {
            var canvas = GameObject.Find("Canvas");
            var buttons = canvas.GetComponentsInChildren<Button>();
            foreach (var btn in buttons) {
                btn.interactable = value;
            }
        }

        public bool GameOver () {
            return currentRound > GameData.Instance.GetGameLength();
        }

        private bool IsGameFull () {
            return players.Count == GameData.Instance.GetRoomSize();
        }

        public void LaunchMainMenu () {
            var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
            if (isServer) {
                nm.StopHost();
            } else {
                //nm.matchMaker.DropConnection(nm.matchInfo.networkId, nm.matchInfo.nodeId, 0, nm.OnDropConnection);
                nm.StopClient();
            }
        }
    }
}
