using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class LineLog : MonoBehaviour
{
    private List<Line> lines = new List<Line>();
    private Text text;

    private void Awake()
    {
        text = GetComponentInChildren<Text>();
    }

    internal void AddNewLine(Line line)
    {
        if (lines.Count == 6)
        {
            lines.Clear();
            text.text = "";
        }

        lines.Add(line);

        if(lines.Count > 1)
            text.text = text.text += "\n" + line.ToString();
        else
            text.text = line.ToString();
    }
}
