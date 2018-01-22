using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineLog : MonoBehaviour
{
    private Text plyerLines;
    private Text playerLabel;
    private List<Line> lines = new List<Line>();

    public string PlayerName
    {
        get { return playerLabel.text; }
        set { playerLabel.text = value; }
    }

    public int LinesCount()
    {
        return lines.Count;
    }

    public string GetLinesAsString()
    {
        return plyerLines.text;
    }

    private void Awake()
    {
        var gos = GetComponentsInChildren<Text>();
        foreach (var go in gos)
        {
            if (go.name == "Log")
                plyerLines = go;
            else if (go.name == "Label")
                playerLabel = go;
        }
    }

    public void AddLine(Line line)
    {
        if (lines.Count > 0)
            plyerLines.text = plyerLines.text + "\n" + line.ToString();
        else
            plyerLines.text = line.ToString();

        lines.Add(line);
    }
}
