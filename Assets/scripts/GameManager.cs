using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;

public class GameManager : NetworkBehaviour
{
    public LineLog p1Log;
    public LineLog p2Log;
    public WordSeaManager wordSea;
    public int wordsPerLine = 4;
    public int linesPerGame = 3;
    public int expectedPlayerCount = 2;
    public GameOverScreen gameOverScreen;
    public ButtonBar buttonBar;

    private List<int> scores;
    // Mapping of client NetIds to the selected words of that client's Player
    // The order of players in this list defines player number, i.e. player at index 0 is player1
    private List<PlayerStrings> playersWords = new List<PlayerStrings>();
    private List<string> playerNames = new List<string>();

    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        scores = new List<int>();
    }

    [ClientRpc]
    void RpcOnAllPlayersJoined(string msg)
    {
        var names = msg.Split(' ');
        playerNames.Clear();
        playerNames.AddRange(names);

        var localPlayer = GetLocalPlayerId();
        if (names[0].Equals(localPlayer))
        {
            p1Log.PlayerName = playerNames[0];
            p2Log.PlayerName = playerNames[1];
        }
        else
        {
            p1Log.PlayerName = playerNames[1];
            p2Log.PlayerName = playerNames[0];
        }
    }

    internal void AddPlayerName(string name)
    {
        playerNames.Add(name);
        if (playerNames.Count == expectedPlayerCount)
        {
            var names = String.Join(" ", playerNames.ToArray());
            RpcOnAllPlayersJoined(names);
        }
    }

    // todo move to AI players "class"?
    //var p2Words = wordSea.PickRandomWordsFromSea(linesInAGame);
    //foreach (var w in p2Words)
    //{
    //    Player2.words.Add(w);
    //}

    [Command]
    internal void CmdLineChosen(string wordPackageJson)
    {
        var wordPkg = JsonUtility.FromJson<PlayerStrings>(wordPackageJson);
        playersWords.Add(wordPkg);

        if (!AllPlayersReady())
            return;

        var playersWordsJson = JsonArrayHelper.ToJson(playersWords.ToArray());
        RpcAllPlayersWordsChosen(playersWordsJson);
    }

    private string GetLocalPlayerId()
    {
        var gos = GameObject.FindGameObjectsWithTag("Player");
        foreach (var go in gos)
        {
            var ni = go.GetComponent<NetworkIdentity>();
            if (ni.isLocalPlayer)
                return ni.netId.ToString();
        }
        return null;
    }

    [ClientRpc]
    void RpcAllPlayersWordsChosen(string str)
    {
        var playersWordsJson = JsonArrayHelper.FromJson<PlayerStrings>(str);
        playersWords.Clear();
        playersWords.AddRange(playersWordsJson);

        //todo move local player to index 0
        List<PlayerStrings> wordColors = DetermineColors();
        scores.Add(DetermineScore(wordColors));
        ShowLines(wordColors);
    }

    private int DetermineScore(List<PlayerStrings> wordColors)
    {
        int score = 0;
        foreach (var item in wordColors[0].strings)
        {
            if (item == "green")
                score += 2;
            else if (item == "yellow")
                score += 1;
        }
        if (PerfectScore(wordsPerLine, score))
            score += 3;
        return score;
    }

    private List<PlayerStrings> DetermineColors()
    {
        string[] p1Words = (string[])playersWords[0].strings.Clone();
        string[] p1Colors = GetInitialColors(wordsPerLine);

        string[] p2Words = (string[])playersWords[1].strings.Clone();
        string[] p2Colors = GetInitialColors(wordsPerLine);

        for (int i = 0; i < wordsPerLine; i++)
        {
            if (p1Words[i].Equals(p2Words[i]))
            {
                p1Colors[i] = "green";
                p2Colors[i] = "green";
                p1Words[i] = null;
                p2Words[i] = null;
            }
        }
        for (int i = 0; i < wordsPerLine; i++)
        {
            if (p1Words[i] != null)
            {
                for (int j = 0; j < wordsPerLine; j++)
                {
                    if (p1Words[i].Equals(p2Words[j]))
                    {
                        p1Colors[i] = "yellow";
                        p2Colors[j] = "yellow";
                        p2Words[j] = null;
                    }
                }
            }
        }
        var wordColors = new List<PlayerStrings>() {
            new PlayerStrings() { id = playersWords[0].id, strings = p1Colors },
            new PlayerStrings() { id = playersWords[1].id, strings = p2Colors }
        };
        return wordColors;
    }

    private static string[] GetInitialColors(int length)
    {
        return Enumerable.Repeat("red", length).ToArray();
    }

    private void ShowLines(List<PlayerStrings> wordColors)
    {
        int playerIx = 0;
        var localPlayerName = GetLocalPlayerId();
        for (; playerIx < playersWords.Count; playerIx++)
        {
            if (playersWords[playerIx].id.Equals(localPlayerName))
            {
                break;
            }
        }

        p1Log.AddLine(new Line(playersWords[playerIx].strings, wordColors[playerIx].strings));
        playerIx = (playerIx + 1) % playersWords.Count;
        p2Log.AddLine(new Line(playersWords[playerIx].strings, wordColors[playerIx].strings));

        if (GameOver())
        {
            MakeUINonInteractable();

            var canvas = GameObject.Find("Canvas");
            var go = Instantiate(gameOverScreen, canvas.transform);
            go.AddData(p1Log.PlayerName, p2Log.PlayerName, p1Log.GetLinesAsString(), p2Log.GetLinesAsString(), scores.ToArray());
        }
        else
        {
            buttonBar.Reset();
            wordSea.Reset();
        }
    }
    private bool AllPlayersReady()
    {
        return playersWords.Count == expectedPlayerCount;
    }

    private void MakeUINonInteractable()
    {
        var canvas = GameObject.Find("Canvas");
        var buttons = canvas.GetComponentsInChildren<Button>();
        foreach (var btn in buttons)
        {
            btn.interactable = false;
        }
    }

    public bool GameOver()
    {
        return p1Log.LinesCount() == linesPerGame && p2Log.LinesCount() == linesPerGame;
    }

    private static bool PerfectScore(int wordCount, int score)
    {
        return score == wordCount * 2;
    }
}
