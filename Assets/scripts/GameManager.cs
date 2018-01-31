﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour {
    public WordSea wordSea;
    public ButtonBar buttonBar;

    private const int wordsPerLine = 4;
    private List<LineLog> lineLogs = new List<LineLog>();
    private List<PlayerConnection> players;
    private ColorMapper colorWordMapper = new ColorMapper(wordsPerLine);

    private static int expectedPlayerCount = 1;
    private static int linesPerGame = 3;

    public static int ExpectedPlayerCount {
        get { return expectedPlayerCount; }
        set { expectedPlayerCount = value; }
    }

    public static int LinesPerGame {
        get { return linesPerGame; }
        set { linesPerGame = value; }
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
        var newWordSea = wordSea.GenerateNewSea();
        RpcOnAllPlayersJoined(newWordSea, expectedPlayerCount);
    }

    [ClientRpc]
    private void RpcOnAllPlayersJoined (string[] newWordSea, int playercount) {
        expectedPlayerCount = playercount;
        players = FindSortedPlayers();

        CreateLineLogs();
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
        colorWordMapper.ComputeScore(0);
        AddWordsToLineLogs(colorWordMapper.GetColors());

        if (GameOver()) {
            SetUIButtonsInteractable(false);
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
        var canvas = GameObject.Find("Canvas");
        var go = Instantiate((GameObject) Resources.Load("GameOverScreen"), canvas.transform);
        var gos = go.GetComponent<GameOverScreen>();

        switch (players.Count) {
            case 1:
                gos.AddData(lineLogs[0].PlayerName, " ",
                    lineLogs[0].GetLinesAsString(), " ", colorWordMapper.GetScores());
                break;
            case 2:
            case 3:
            case 4:
                gos.AddData(lineLogs[0].PlayerName, lineLogs[1].PlayerName,
                    lineLogs[0].GetLinesAsString(), lineLogs[1].GetLinesAsString(),
                    colorWordMapper.GetScores());
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
            if (ll.LinesCount() != LinesPerGame)
                return false;
        }
        return true;
    }

    private bool IsGameFull () {
        return players.Count == ExpectedPlayerCount;
    }

    public void LaunchMainMenu () {
        //    SceneManager.LoadScene("main_menu");
    }
}
