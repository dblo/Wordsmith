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
    string[] library2 = new string[] { "is", "are", "you", "what", "when", "like", "a", "your" };

    internal void ReturnWord(Button btn)
    {
        var text = btn.GetComponentInChildren<Text>();
        SetTextAlpha(text, 1f);
        btn.interactable = true;
    }

    public void WordClicked(Button btn)
    {
        var text = btn.GetComponentInChildren<Text>();
        if (buttonBar.TryAdd(btn))
        {
            SetTextAlpha(text, 0f);
            btn.interactable = false;
        }
    }

    private void SetTextAlpha(Text text, float alpha)
    {
        var c = text.color;
        var newColor = new Color(c.r, c.g, c.b, alpha);
        text.color = newColor;
    }

    internal void SetNewSea(string[] words)
    {
        currentSea.Clear();

        for (int i = 0; i < words.Length; i++)
        {
            var text = buttons[i].GetComponentInChildren<Text>();
            text.text = words[i];
            SetTextAlpha(text, 1f);
            currentSea.Add(words[i]);
            buttons[i].interactable = true;
        }
    }

    internal string[] GenerateNewSea()
    {
        var words1 = GenerateUniqueWords(buttons.Count - 3, library);
        var words2 = GenerateUniqueWords(3, library2);
        string[] words = new string[words1.Length + words2.Length];
        words1.CopyTo(words, 0);
        words2.CopyTo(words, words1.Length);
        return words;
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
