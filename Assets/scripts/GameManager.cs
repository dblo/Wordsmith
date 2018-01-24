using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public List<LineLog> lineLogs = new List<LineLog>();
    public WordSeaManager wordSea;
    public int wordsPerLine = 4;
    public int linesPerGame = 3;
    public int expectedPlayerCount = 2;
    public GameOverScreen gameOverScreen;
    public ButtonBar buttonBar;
    public GameObject pauseMenuPrefab;
    public GameObject lineLogPrefab;

    private List<int> scores = new List<int>();
    // Mapping of client NetIds to the selected words of that client's Player
    // The order of players in this list defines player number, i.e. player at index 0 is player1
    private List<PlayerStrings> playersWords = new List<PlayerStrings>();
    private List<string> playerNames = new List<string>();

    private void Awake()
    {
        if (GameStatics.PlayerCount > 0)
            expectedPlayerCount = GameStatics.PlayerCount;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    [Command]
    internal void CmdAddPlayerName(string name)
    {
        playerNames.Add(name);
        if (IsGameFull())
        {
            var data = new SetupData()
            {
                playerNames = playerNames.ToArray(),
                wordSeaWords = wordSea.GenerateNewSea()
            };
            var dataJson = JsonUtility.ToJson(data);
            RpcOnAllPlayersJoined(dataJson);
        }
    }

    [ClientRpc]
    void RpcOnAllPlayersJoined(string setupDataJson)
    {
        var setupData = JsonUtility.FromJson<SetupData>(setupDataJson);
        playerNames.Clear();
        playerNames.AddRange(setupData.playerNames);

        var localPlayerName = GetLocalPlayerId();
        int playerIx = 0;
        for (; playerIx < playerNames.Count; playerIx++)
        {
            if (playerNames[playerIx].Equals(localPlayerName))
                break;
        }

        switch (playerNames.Count)
        {
            case 1:
                lineLogs.Add(CreateLineLog(0f, 1f, localPlayerName));
                break;
            case 2:
                lineLogs.Add(CreateLineLog(0f, 0.5f, localPlayerName));
                playerIx = (playerIx + 1) % playerNames.Count;
                lineLogs.Add(CreateLineLog(0.5f, 1f, playerNames[playerIx]));
                break;
            default:
                throw new ArgumentException();
        }
        wordSea.SetNewSea(setupData.wordSeaWords);
    }

    LineLog CreateLineLog(float anchorXMin, float anchorXMax, string playerName)
    {
        var anchorYMin = 0.65f;
        var anchorYMax = 1f;
        Transform canvasTrans = GameObject.Find("Canvas").transform;
        var go = Instantiate(lineLogPrefab, canvasTrans, false);
        var rTrans = go.GetComponent<RectTransform>();
        rTrans.anchorMin = new Vector2(anchorXMin, anchorYMin);
        rTrans.anchorMax = new Vector2(anchorXMax, anchorYMax);

        var ll = go.GetComponent<LineLog>();
        ll.PlayerName = playerName;
        return ll;
    }

    [Command]
    internal void CmdLineChosen(string wordPackageJson)
    {
        var wordPkg = JsonUtility.FromJson<PlayerStrings>(wordPackageJson);
        playersWords.Add(wordPkg);

        if (AllPlayersReady())
        {
            ConcludeRoundAndTellClients();
        }
    }

    private void ConcludeRoundAndTellClients()
    {
        var nextWordSea = GameOver() ? null : wordSea.GenerateNewSea();
        var roundData = new RoundData()
        {
            strings = playersWords.ToArray(),
            wordSeaWords = nextWordSea
        };

        var playersWordsJson = JsonUtility.ToJson(roundData);
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

    // Return index of localplayer in the playerWords list
    private int GetLocalPlayerIndex()
    {
        var localPlayerName = GetLocalPlayerId();
        int playerIx = 0;
        for (; playerIx < playersWords.Count; playerIx++)
        {
            if (playersWords[playerIx].id.Equals(localPlayerName))
                break;
        }
        return playerIx;
    }

    [ClientRpc]
    void RpcAllPlayersWordsChosen(string roundDataJson)
    {
        var roundData = JsonUtility.FromJson<RoundData>(roundDataJson);
        playersWords.Clear();
        playersWords.AddRange(roundData.strings);

        List<PlayerStrings> wordColors;
        if (playerNames.Count == 1)
        {
            wordColors = new List<PlayerStrings>() {
                new PlayerStrings() { id = playersWords[0].id, strings = GetInitialColors(wordsPerLine) }
            };
        }
        else if (playerNames.Count == 2)
        {
            wordColors = DetermineColors();
        }
        else
            throw new ArgumentException();

        scores.Add(DetermineScore(wordColors));
        ShowLines(wordColors);

        if (GameOver())
        {
            SetUIButtonsInteractable(false);
            ShowGameOverScreen();
        }
        else
        {
            playersWords.Clear();
            buttonBar.Reset();
            wordSea.SetNewSea(roundData.wordSeaWords);
        }
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

    private void ShowLines(List<PlayerStrings> wordColors)
    {
        var playerIx = GetLocalPlayerIndex();
        for (int i = 0; i < playersWords.Count; i++)
        {
            lineLogs[i].AddLine(new Line(playersWords[playerIx].strings, wordColors[playerIx].strings));
            playerIx = (playerIx + 1) % playersWords.Count;
        }
    }

    private void ShowGameOverScreen()
    {
        var canvas = GameObject.Find("Canvas");
        var go = Instantiate(gameOverScreen, canvas.transform);

        switch (playerNames.Count)
        {
            case 1:
                go.AddData(lineLogs[0].PlayerName, " ",
                    lineLogs[0].GetLinesAsString(), " ", scores.ToArray());
                break;
            case 2:
                go.AddData(lineLogs[0].PlayerName, lineLogs[1].PlayerName,
                    lineLogs[0].GetLinesAsString(), lineLogs[1].GetLinesAsString(),
                    scores.ToArray());
                break;
            default:
                throw new ArgumentException();
        }
    }

    private bool AllPlayersReady()
    {
        return playersWords.Count == expectedPlayerCount;
    }

    internal void SetUIButtonsInteractable(bool value)
    {
        var canvas = GameObject.Find("Canvas");
        var buttons = canvas.GetComponentsInChildren<Button>();
        foreach (var btn in buttons)
        {
            btn.interactable = value;
        }
    }

    public bool GameOver()
    {
        foreach (var ll in lineLogs)
        {
            if (ll.LinesCount() != linesPerGame)
                return false;
        }
        return true;
    }

    // True if score is such that 2 points per word was awarded
    private static bool PerfectScore(int wordCount, int score)
    {
        return score == wordCount * 2;
    }
    
    private bool IsGameFull()
    {
        return playerNames.Count == expectedPlayerCount;
    }

    internal void LaunchMainMenu()
    {
        SceneManager.LoadScene("main_menu");
    }
}
