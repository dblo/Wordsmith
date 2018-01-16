using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordSeaManager : MonoBehaviour
{
    public ButtonBar buttonBar;
    public List<Button> buttons;

    private List<string> currentSea = new List<string>();

    string[] library = new string[]{ "copper","explain", "ill-fated", "truck", "neat",
                                         "unite", "branch", "educated", "tenuous",
                                         "hum", "decisive", "notice", "yell",
                                         "parched", "attend", "punish", "push",
                                         "pat", "zephyr", "retire", "anxious",
                                         "trade", "suspend", "train", "brash",
                                         "insurance", "groan", "remarkable", "print",
                                         "cow", "wrench", "tremble", "surround",
                                         "same", "double", "road" };

    internal void ReturnWord(Button btn)
    {
        var text = btn.GetComponentInChildren<Text>();
        UseDefaultColor(text);
        btn.interactable = true;
    }

    private static void UseDefaultColor(Text text)
    {
        text.color = new Color(50f / 255, 50f / 255, 50f / 255, 255);
    }

    private void Start()
    {
        Reset();
    }

    public void WordClicked(Button btn)
    {
        var text = btn.GetComponentInChildren<Text>();
        if (buttonBar.TryAdd(btn))
        {
            UseTransparantColor(text);
            btn.interactable = false;
        }
    }

    private static void UseTransparantColor(Text text)
    {
        text.color = new Color(0, 0, 0, 0);
    }

    internal void Reset()
    {
        currentSea.Clear();
        var words = GenerateUniqueWords(buttons.Count, library);

        for (int i = 0; i < words.Length; i++)
        {
            var text = buttons[i].GetComponentInChildren<Text>();
            text.text = words[i];
            UseDefaultColor(text); 
            currentSea.Add(words[i]);
            buttons[i].interactable = true;
        }
    }

    public string[] PickRandomWordsFromSea(int count)
    {
        return GenerateUniqueWords(count, currentSea.ToArray());
    }

    private string[] GenerateUniqueWords(int aSize, string[] aLibrary)
    {
        HashSet<int> indices = new HashSet<int>();
        string[] words = new string[aSize];

        System.Random rng = new System.Random();
        for (int i = 0; i < aSize; i++)
        {
            int r = rng.Next(aLibrary.Length);
            while (indices.Contains(r))
            {
                r = rng.Next(aLibrary.Length);
            }
            indices.Add(r);
            words[i] = aLibrary[r];
        }
        return words;
    }
}
