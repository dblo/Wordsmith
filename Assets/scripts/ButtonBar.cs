using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBar : MonoBehaviour
{
    public Button[] buttons;
    public LineLog linelog;
    private int currentButton = 0;
    public WordSeaManager wordSea;

    internal bool TryAdd(string v)
    {
        if (AllWordsChosen())
            return false;

        var text = buttons[currentButton].GetComponentInChildren<Text>();
        text.text = v;
        currentButton++;
        return true;
    }

    internal void Reset()
    {
        foreach (var btn in buttons)
        {
            btn.GetComponentInChildren<Text>().text = "";
        }
        currentButton = 0;
    }

    public void TryAcceptLine()
    {
        if (AllWordsChosen())
        {
            linelog.AddNewLine(MakeLine());
            Reset();
            wordSea.Reset();
        }
    }

    Line MakeLine()
    {
        List<string> words = new List<string>(buttons.Length);
        foreach (var button in buttons)
        {
            words.Add(button.GetComponentInChildren<Text>().text);
        }
        return new Line(words);
    }

    private bool AllWordsChosen()
    {
        return currentButton == buttons.Length;
    }
}
