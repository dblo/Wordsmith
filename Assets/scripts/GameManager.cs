using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class GameManager : NetworkBehaviour {
    public WordSea wordSea;
    public ButtonBar buttonBar;
    public ScorePanel scorePanelPrefab;

    private List<LineLog> lineLogs = new List<LineLog>();
    private List<PlayerConnection> players;
    private ColorMapper colorWordMapper;
    private static int expectedPlayerCount = 1;
    private static int linesPerGame = 3;

    public bool SoundMuted { get; private set; }

    public static int ExpectedPlayerCount {
        get { return expectedPlayerCount; }
        set { expectedPlayerCount = value; }
    }

    public static int LinesPerGame {
        get { return linesPerGame; }
        set { linesPerGame = value; }
    }

    private void Awake () {
        SoundMuted = PlayerPrefs.GetInt("SoundMuted", 0) > 0;
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
        var newWordSea = wordSea.GenerateNewSea();
        RpcOnAllPlayersJoined(newWordSea, expectedPlayerCount, LinesPerGame, ButtonBar.lineLength);
    }

    public void SetSoundMuted (bool value) {
        SoundMuted = value;
    }

    [ClientRpc]
    private void RpcOnAllPlayersJoined (string[] newWordSea, int playercount, int gameLength, int lineLength) {
        expectedPlayerCount = playercount;
        LinesPerGame = gameLength;
        ButtonBar.lineLength = lineLength;
        WordSea.wordSeaSize = newWordSea.Length;
        players = FindSortedPlayers();
        colorWordMapper = new ColorMapper(playercount);

        CreateLineLogs();
        wordSea.SetNewSea(newWordSea);
        wordSea.ConfigureSea();
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

        if (GameOver()) {
            buttonBar.OnGameOver();
            ShowGameOverScreen();
        } else {
            foreach (var p in players) {
                p.Reset();
            }
            buttonBar.Reset();
            wordSea.SetNewSea(newWordSea);
        }
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
        foreach (var ll in lineLogs) {
            if (ll.LinesCount() != LinesPerGame)
                return false;
        }
        return true;
    }

    private bool IsGameFull () {
        return players.Count == ExpectedPlayerCount;
    }

    public void LaunchMainMenu () {
        var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        if (isServer)
            nm.StopHost();
        else
            nm.StopClient();
    }
}
