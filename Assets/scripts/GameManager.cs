using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public int linesPerGame;
    public WordSea wordSea;
    public ButtonBar buttonBar;
    public GameOverScreen gameOverPrefab;
    public LineLog lineLogPrefab;

    private const int wordsPerLine = 4;
    private const int perfectScoreBonus = 2;
    private List<LineLog> lineLogs = new List<LineLog>();
    private List<int> scores = new List<int>();
    // Mapping of client NetIds to the selected words of that client's Player
    // The order of players in this list defines player number, i.e. player at index 0 is player1
    private PlayerStrings[] playersWords;

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public override void OnStartServer()
    {
        playersWords = new PlayerStrings[MyNetworkManager.PlayerCount];
    }

    [Command]
    internal void CmdAddPlayer(string id)
    {
        for (int i = 0; i < playersWords.Length; i++)
        {
            if (playersWords[i] == null)
            {
                playersWords[i] = new PlayerStrings()
                {
                    playerID = id,
                    strings = null
                };
                break;
            }
        }
        if (IsGameFull())
        {
            var playerIDs = new string[playersWords.Length];
            for (int i = 0; i < playersWords.Length; i++)
            {
                playerIDs[i] = playersWords[i].playerID;
            }
            var playerNames = GetPlayersNames();
            var data = new SetupData()
            {
                playerIDs = playerIDs,
                playerNames = playerNames,
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
        int localPlayerIndex = 0;

        playersWords = new PlayerStrings[MyNetworkManager.PlayerCount];

        for (int i = 0; i < playersWords.Length; i++)
        {
            playersWords[i] = new PlayerStrings()
            {
                playerID = setupData.playerIDs[i],
                strings = null
            };
            if (setupData.playerIDs[i].Equals(GetLocalPlayeID()))
                localPlayerIndex = i;
        }

        MoveLocalPlayerToFront(localPlayerIndex);
        MoveLocalPlayerToFront(localPlayerIndex, ref setupData.playerNames);

        CreateLineLogs(setupData.playerNames);
        wordSea.SetNewSea(setupData.wordSeaWords);
    }

    private void CreateLineLogs(string[] playerNames)
    {
        switch (playersWords.Length)
        {
            case 1:
                lineLogs.Add(CreateLineLog(0f, 1f, playerNames[0]));
                break;
            case 2:
                lineLogs.Add(CreateLineLog(0f, 0.5f, playerNames[0]));
                lineLogs.Add(CreateLineLog(0.5f, 1f, playerNames[1]));
                break;
            default:
                throw new ArgumentException();
        }
    }

    // Returns an array of player names sorted according to current order of playersWords
    private string[] GetPlayersNames()
    {
        var playerNames = new string[playersWords.Length];
        HashSet<GameObject> players = new HashSet<GameObject>(GameObject.FindGameObjectsWithTag("Player"));

        for (int i = 0; i < playersWords.Length; i++)
        {
            var iter = players.GetEnumerator();
            while (iter.MoveNext())
            {
                var nb = iter.Current.GetComponent<NetworkBehaviour>();
                if (nb.netId.Value.ToString().Equals(playersWords[i].playerID))
                {
                    playerNames[i] = iter.Current.GetComponent<PlayerConnection>().playerName;
                    players.Remove(iter.Current);
                    break;
                }
            }
        }
        return playerNames;
    }

    private void MoveLocalPlayerToFront(int localPlayerIndex)
    {
        PlayerStrings tmp = playersWords[0];
        playersWords[0] = playersWords[localPlayerIndex];
        playersWords[localPlayerIndex] = tmp;
    }

    private void MoveLocalPlayerToFront(int localPlayerIndex, ref string[] arr)
    {
        string tmp = arr[0];
        arr[0] = arr[localPlayerIndex];
        arr[localPlayerIndex] = tmp;
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
        for (int i = 0; i < playersWords.Length; i++)
        {
            if (playersWords[i].playerID.Equals(wordPkg.playerID))
            {
                playersWords[i] = wordPkg;
                break;
            }
        }
        if (AllPlayersReady())
        {
            DistributeRoundData();
        }
    }

    private void DistributeRoundData()
    {
        var nextWordSea = GameOver() ? null : wordSea.GenerateNewSea();
        var roundData = new RoundData()
        {
            playerStrings = playersWords,
            wordSeaWords = nextWordSea
        };
        var playersWordsJson = JsonUtility.ToJson(roundData);
        ClearPlayersWords(); // Let's us treat host as any client in RpcAllPlayersWordsChosen()
        RpcAllPlayersWordsChosen(playersWordsJson);
    }

    private void ClearPlayersWords()
    {
        for (int i = 0; i < playersWords.Length; i++)
        {
            playersWords[i].strings = null;
        }
    }

    private string GetLocalPlayeID()
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
    void RpcAllPlayersWordsChosen(string roundDataJson)
    {
        var roundData = JsonUtility.FromJson<RoundData>(roundDataJson);
        // todo instead of below, do the one swap in roundData so it is parallell to playerWords and copy
        for (int i = 0; i < roundData.playerStrings.Length; i++)
        {
            for (int j = 0; j < playersWords.Length; j++)
            {
                if (roundData.playerStrings[i].playerID.Equals(playersWords[j].playerID))
                {
                    playersWords[j] = roundData.playerStrings[i];
                    break;
                }
            }
        }
        List<PlayerStrings> wordColors;
        switch (playersWords.Length)
        {
            case 1:
                wordColors = new List<PlayerStrings>() {
                new PlayerStrings() { playerID = playersWords[0].playerID, strings = GetInitialColors(wordsPerLine) }
            };
                break;
            case 2:
                wordColors = Determine2PlayerColors();
                break;
            default:
                throw new ArgumentException();
        }

        scores.Add(DetermineScore(wordColors));
        ShowLines(wordColors);

        if (GameOver())
        {
            SetUIButtonsInteractable(false);
            ShowGameOverScreen();
        }
        else
        {
            ClearPlayersWords();
            buttonBar.Reset();
            wordSea.SetNewSea(roundData.wordSeaWords);
        }
    }

    private List<PlayerStrings> Determine2PlayerColors()
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
            new PlayerStrings() { playerID = playersWords[0].playerID, strings = p1Colors },
            new PlayerStrings() { playerID = playersWords[1].playerID, strings = p2Colors }
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
            score += perfectScoreBonus;
        return score;
    }

    private void ShowLines(List<PlayerStrings> wordColors)
    {
        for (int i = 0; i < playersWords.Length; i++)
        {
            lineLogs[i].AddLine(new Line(playersWords[i].strings, wordColors[i].strings));
        }
    }

    private void ShowGameOverScreen()
    {
        var canvas = GameObject.Find("Canvas");
        var go = Instantiate(gameOverPrefab, canvas.transform);

        switch (playersWords.Length)
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
        foreach (var p in playersWords)
        {
            if (p.strings == null)
                return false;
        }
        return true;
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
        foreach (var p in playersWords)
        {
            if (p == null)
                return false;
        }
        return true;
    }

    internal void LaunchMainMenu()
    {
        SceneManager.LoadScene("main_menu");
    }
}
