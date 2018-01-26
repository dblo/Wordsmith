using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour {
    public int linesPerGame;
    public WordSea wordSea;
    public ButtonBar buttonBar;
    public GameOverScreen gameOverPrefab;
    public LineLog lineLogPrefab;

    private const int wordsPerLine = 4;
    private const int perfectScoreBonus = 2;
    private List<LineLog> lineLogs = new List<LineLog>();
    private List<int> lineScores = new List<int>();
    private List<PlayerConnection> players;

    public override void OnStartServer () {
        players = new List<PlayerConnection>();
    }

    // Only called on server
    public void PlayerSetupDone (PlayerConnection player) {
        players.Add(player);
        if (!IsGameFull())
            return;

        players.ForEach((p) => p.CmdSynchronizeName());
        var newWordSea = String.Join(" ", wordSea.GenerateNewSea());
        RpcOnAllPlayersJoined(newWordSea);
    }

    [ClientRpc]
    private void RpcOnAllPlayersJoined (string newWordSea) {
        players = FindSortedPlayers();
        CreateLineLogs();
        wordSea.SetNewSea(newWordSea.Split(' '));
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
        switch (players.Count) {
            case 1:
                lineLogs.Add(CreateLineLog(0f, 1f, players[0].PlayerName));
                break;
            case 2:
                lineLogs.Add(CreateLineLog(0f, 0.5f, players[0].PlayerName));
                lineLogs.Add(CreateLineLog(0.5f, 1f, players[1].PlayerName));
                break;
            default:
                throw new ArgumentException();
        }
    }

    private LineLog CreateLineLog (float anchorXMin, float anchorXMax, string playerName) {
        const float anchorYMin = 0.65f;
        const float anchorYMax = 1f;
        var canvasTrans = GameObject.Find("Canvas").transform;
        var go = Instantiate(lineLogPrefab, canvasTrans, false);
        var rTrans = go.GetComponent<RectTransform>();
        rTrans.anchorMin = new Vector2(anchorXMin, anchorYMin);
        rTrans.anchorMax = new Vector2(anchorXMax, anchorYMax);

        var ll = go.GetComponent<LineLog>();
        ll.PlayerName = playerName;
        return ll;
    }

    [Command]
    public void CmdPlayerReady () {
        if (!AllPlayersReady())
            return;

        players.ForEach((p) => p.CmdSynchronizeWords());
        var newWordSea = GameOver() ? null : wordSea.GenerateNewSea();
        RpcAllPlayersReady(String.Join(" ", newWordSea));
    }

    [ClientRpc]
    void RpcAllPlayersReady (string newWordSea) {
        var wordColors = ComputeWordColors();
        AddWordsToLineLogs(wordColors);

        var score = ComputeScore(wordColors[0]);  // Could use any player's colors
        lineScores.Add(score);

        if (GameOver()) {
            SetUIButtonsInteractable(false);
            ShowGameOverScreen();
        } else {
            foreach (var p in players) {
                p.Reset();
            }
            buttonBar.Reset();
            wordSea.SetNewSea(newWordSea.Split(' '));
        }
    }

    // Returns list of colors where element i correspond to the words for player i in member players
    private List<string[]> ComputeWordColors () {
        List<string[]> wordColors;
        switch (players.Count) {
            case 1:
                wordColors = new List<string[]>() { GetInitialColors(wordsPerLine) };
                break;
            case 2:
                wordColors = Determine2PlayerColors();
                break;
            default:
                throw new ArgumentException();
        }

        return wordColors;
    }

    private List<string[]> Determine2PlayerColors () {
        string[] p1Words = (string[]) players[0].Words.Clone();
        string[] p2Words = (string[]) players[1].Words.Clone();
        string[] p1Colors = GetInitialColors(wordsPerLine);
        string[] p2Colors = (string[]) p1Colors.Clone();

        for (int i = 0; i < wordsPerLine; i++) {
            if (p1Words[i].Equals(p2Words[i])) {
                p1Colors[i] = "green";
                p2Colors[i] = "green";
                p1Words[i] = null;
                p2Words[i] = null;
            }
        }
        for (int i = 0; i < wordsPerLine; i++) {
            if (p1Words[i] != null) {
                for (int j = 0; j < wordsPerLine; j++) {
                    if (p1Words[i].Equals(p2Words[j])) {
                        p1Colors[i] = "yellow";
                        p2Colors[j] = "yellow";
                        p2Words[j] = null;
                    }
                }
            }
        }
        return new List<string[]>() { p1Colors, p2Colors };
    }

    private static string[] GetInitialColors (int length) {
        return Enumerable.Repeat("red", length).ToArray();
    }

    private int ComputeScore (string[] colors) {
        int score = 0;
        foreach (var c in colors) {
            if (c == "green")
                score += 2;
            else if (c == "yellow")
                score += 1;
        }
        if (PerfectScore(wordsPerLine, score))
            score += perfectScoreBonus;
        return score;
    }

    private void AddWordsToLineLogs (List<string[]> colors) {
        for (int i = 0; i < players.Count; i++) {
            lineLogs[i].AddLine(players[i].Words, colors[i]);
        }
    }

    private void ShowGameOverScreen () {
        var canvas = GameObject.Find("Canvas");
        var go = Instantiate(gameOverPrefab, canvas.transform);

        switch (players.Count) {
            case 1:
                go.AddData(lineLogs[0].PlayerName, " ",
                    lineLogs[0].GetLinesAsString(), " ", lineScores.ToArray());
                break;
            case 2:
                go.AddData(lineLogs[0].PlayerName, lineLogs[1].PlayerName,
                    lineLogs[0].GetLinesAsString(), lineLogs[1].GetLinesAsString(),
                    lineScores.ToArray());
                break;
            default:
                throw new ArgumentException();
        }
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
            if (ll.LinesCount() != linesPerGame)
                return false;
        }
        return true;
    }

    // True if score is such that 2 points per word was awarded
    private static bool PerfectScore (int wordCount, int score) {
        return score == wordCount * 2;
    }

    private bool IsGameFull () {
        return players.Count == MyNetworkManager.ExpectedPlayerCount;
    }

    public void LaunchMainMenu () {
        SceneManager.LoadScene("main_menu");
    }
}
