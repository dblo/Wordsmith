using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineLog : MonoBehaviour
{
    private int maxVisibleRows;
    private Text text;
    private List<Line> lines = new List<Line>();

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    public void SetLinesInAGame(int count)
    {
        maxVisibleRows = count;
    }

    public void AddLine(Line line)
    {
        if (lines.Count + 1 > maxVisibleRows)
            throw new InvalidOperationException();

        if (lines.Count > 0)
            text.text = text.text + "\n" + line.ToString();
        else
            text.text = line.ToString();

        lines.Add(line);
    }
}
