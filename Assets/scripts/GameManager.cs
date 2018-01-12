using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public Text p1text;
    public Text p2text;
    public WordSeaManager wordSea;

    private List<Line> p1Lines = new List<Line>();
    private List<Line> p2Lines = new List<Line>();

    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    internal void AddNewLine(Line line, Player player)
    {
        AddLine(line, player);
        AddLine(new Line(new List<string>(wordSea.PickRandomWordsFromSea(line.Length))), Player.Player2);
    }

    private void AddLine(Line line, Player player)
    {
        Text playerText = player == Player.Player1 ? p1text : p2text;
        List<Line> playerLines = player == Player.Player1 ? p1Lines: p2Lines;

        if (playerLines.Count == 6)
        {
            playerLines.Clear();
            playerText.text = "";
        }

        playerLines.Add(line);

        if (playerLines.Count > 1)
            playerText.text = playerText.text += "\n" + line.ToString();
        else
            playerText.text = line.ToString();
    }
}
