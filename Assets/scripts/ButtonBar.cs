using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBar : MonoBehaviour
{
    public Button[] buttons;
    private Dictionary<Button, Button> buttonMap = new Dictionary<Button, Button>();

    private int currentButton = 0;
    public WordSeaManager wordSea;
    public GameManager gm;

    internal bool TryAdd(Button btn)
    {
        if (AllWordsChosen())
            return false;

        var newText = btn.GetComponentInChildren<Text>();
        var oldText = buttons[currentButton].GetComponentInChildren<Text>();

        oldText.text = newText.text;
        buttonMap.Add(buttons[currentButton], btn);
        currentButton++;
        return true;
    }

    public void WordClicked(Button button)
    {
        Button wordSeaButton;
        if (!buttonMap.TryGetValue(button, out wordSeaButton))
            return; // Clicking a btn without a word. TODO this could hide error if we a re out of sync?

        var text = button.GetComponentInChildren<Text>();
        text.text = "";
        buttonMap.Remove(button);
        wordSea.ReturnWord(wordSeaButton);
        ShiftWordsLeft();
        currentButton--;
    }

    private void ShiftWordsLeft()
    {
        for (int i = 0; i < currentButton - 1; i++)
        {
            if(!buttonMap.ContainsKey(buttons[i]))
            {
                var leftButtonText = buttons[i].GetComponentInChildren<Text>();
                var rightButtonText = buttons[i+1].GetComponentInChildren<Text>();

                leftButtonText.text = rightButtonText.text;
                rightButtonText.text = "";

                Button wordSeaButton;
                if (!buttonMap.TryGetValue(buttons[i+1], out wordSeaButton))
                    throw new InvalidOperationException();
                buttonMap.Remove(buttons[i+1]);
                buttonMap.Add(buttons[i], wordSeaButton);
            }
        }
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
            gm.AddNewLine(MakeLine(), Player.Player1);
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
