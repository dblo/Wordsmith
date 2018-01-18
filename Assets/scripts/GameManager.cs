using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LineLog p1Log;
    public LineLog p2Log;
    public WordSeaManager wordSea;
    public int linesInAGame = 5;
    public GameOverScreen gameOverScreen;
    private List<int> scores;
    
    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        p1Log.SetLinesInAGame(linesInAGame);
        p2Log.SetLinesInAGame(linesInAGame);
        scores = new List<int>();
    }

    internal void LineChosen(string[] words, Player player)
    {
        var p2words = wordSea.PickRandomWordsFromSea(words.Length);
        string[] p1WordsCopy = (string[])words.Clone();
        string[] p2WordsCopy = (string[])p2words.Clone();
        string[] p1Colors = new string[words.Length];
        string[] p2Colors = new string[words.Length];
        int score = 0;

        for (int i = 0; i < p1Colors.Length; i++)
        {
            p1Colors[i] = "red";
            p2Colors[i] = "red";
        }
        for (int i = 0; i < p1WordsCopy.Length; i++)
        {
            if (p1WordsCopy[i].Equals(p2WordsCopy[i]))
            {
                p1Colors[i] = "green";
                p2Colors[i] = "green";
                p1WordsCopy[i] = null;
                p2WordsCopy[i] = null;
                score += 2;
            }
        }
        for (int i = 0; i < p1WordsCopy.Length; i++)
        {
            if (p1WordsCopy[i] != null)
            {
                for (int j = 0; j < p2WordsCopy.Length; j++)
                {
                    if (p1WordsCopy[i].Equals(p2WordsCopy[j]))
                    {
                        p1Colors[i] = "yellow";
                        p2Colors[j] = "yellow";
                        p2WordsCopy[j] = null;
                        score += 1;
                    }
                }
            }
        }
        if (PerfectScore(words.Length, score))
            score += 3;

        p1Log.AddLine(new Line(words, p1Colors));
        p2Log.AddLine(new Line(p2words, p2Colors));
        scores.Add(score);

        if (GameOver())
        {
            var canvas = GameObject.Find("Canvas");
            var go = Instantiate(gameOverScreen, canvas.transform);
            //todo Make everything else non-ineractable
            go.AddData(p1Log.GetLinesAsString(), p2Log.GetLinesAsString(), scores.ToArray());
        }
    }

    private bool GameOver()
    {
        return p1Log.LinesCount() == linesInAGame && p2Log.LinesCount() == linesInAGame;
    }

    private static bool PerfectScore(int wordCount, int score)
    {
        return score == wordCount * 2;
    }
}
