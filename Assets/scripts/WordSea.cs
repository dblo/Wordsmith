using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordSea : MonoBehaviour {
    public static string currentLibraryName = "";
    public static int wordSeaSize = 12;

    public ButtonBar buttonBar;
    public List<Button> buttons;

    private List<string> currentSea = new List<string>();
    public const int seaRows = 3;
    public const int seaCols = 4;
    private const int maxWordSizeSize = 12;
    string[] library = new string[]{ "copper","explain", "ill-fated", "truck", "neat",
                                         "unite", "branch", "educated", "tenuous",
                                         "hum", "decisive", "notice", "yell",
                                         "parched", "attend", "punish", "push",
                                         "pat", "zephyr", "retire", "anxious",
                                         "trade", "suspend", "train", "brash",
                                         "insurance", "groan", "remarkable", "print",
                                         "cow", "wrench", "tremble", "surround",
                                         "same", "double", "road", "is", "are", "you",
                                         "what", "when", "like", "a", "your" };

    private void Start () {
        foreach (var b in buttons) {
            b.onClick.AddListener(delegate { WordClicked(b); });
        }
    }

    public void ReturnWord (Button btn) {
        btn.GetComponent<WordButton>().MoveToWordSea(transform, WordClicked);
    }

    public void WordClicked (Button btn) {
        if(buttonBar.TryAdd(btn)) {
            btn.GetComponent<WordButton>().PlayChoseWordSounds();
        }
    }

    private void SetTextAlpha (Text text, float alpha) {
        var c = text.color;
        var newColor = new Color(c.r, c.g, c.b, alpha);
        text.color = newColor;
    }

    public void SetSeaFrozen (bool value) {
        foreach (var b in buttons) {
            b.interactable = !value;
        }
    }

    public void ConfigureSea() {
        for (int i = wordSeaSize; i < maxWordSizeSize; i++) {
            buttons[i].gameObject.SetActive(false);
        }
    }

    public void SetNewSea (string[] words) {
        currentSea.Clear();
        
        for (int i = 0; i < words.Length; i++) {
            var text = buttons[i].GetComponentInChildren<Text>();
            text.text = words[i];
            currentSea.Add(words[i]);
        }
        SetSeaFrozen(false);
    }

    public string[] GenerateNewSea () {
        if (currentLibraryName == "Default") {
            var newSea = GenerateUniqueWords(wordSeaSize , library);
            return newSea;
        }
        if (currentLibraryName != "") {
            var library = PlayerPrefs.GetString(currentLibraryName);
            if (library == "")
                throw new System.InvalidOperationException("Empty library in WordSea.GenerateNewSea");
            return library.Split(';');
        }
        throw new System.InvalidOperationException("Empty currenLibrary in WordSea.GenerateNewSea");
    }

    public string[] PickRandomWordsFromSea (int count) {
        return GenerateUniqueWords(count, currentSea.ToArray());
    }

    private string[] GenerateUniqueWords (int aSize, string[] aLibrary) {
        HashSet<int> indices = new HashSet<int>();
        string[] words = new string[aSize];

        System.Random rng = new System.Random();
        for (int i = 0; i < aSize; i++) {
            int r = rng.Next(aLibrary.Length);
            while (indices.Contains(r)) {
                r = rng.Next(aLibrary.Length);
            }
            indices.Add(r);
            words[i] = aLibrary[r];
        }
        return words;
    }
}
