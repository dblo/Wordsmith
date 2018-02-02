using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordSea : MonoBehaviour {
    public static string currentLibraryName = "";
    public static int wordSeaSize = 12;
    public Button wordButtonPrefab;
    public ButtonBar buttonBar;
    public const int MaxCols = 4;
    public const int MaxRows = 3;

    private List<Button> buttons;
    private List<string> currentSea;

    string[] nouns = { "time","person","year","way","day","thing","man","world","life","hand","part","child","eye","woman","place","work","week","case","point","company","number" };
    string[] verbs = { "be","have","do","say","get","make","go","know","take","see","come","think","look","want","give","use","find","tell","ask","work","seem","feel","try","leave","call" };
    string[] adjectives = { "good", "first", "new", "last", "long", "great", "little", "own", "other", "old", "right", "big", "high", "different", "small", "large", "next", "early", "young", "important", "few", "public", "bad", "same", "able" };
    string[] prepositions = { "to", "of", "in", "for", "on", "with", "at", "by", "from", "up", "about", "into", "over", "after" };

    private void Awake () {
        buttons = new List<Button>(wordSeaSize);
        currentSea = new List<string>(wordSeaSize);
    }

    public void ReturnWord (Button btn) {
        btn.GetComponent<WordButton>().MoveToWordSea(transform, WordClicked);
    }

    public void WordClicked (Button btn) {
        if (buttonBar.TryAdd(btn)) {
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

    public void ConfigureSea () {
        for (int i = 0; i < wordSeaSize; i++) {
            var btn = Instantiate(wordButtonPrefab, transform);
            var wb = btn.GetComponent<WordButton>();
            wb.ComputeSeaAnchors(i);
            wb.MoveToWordSea(transform, WordClicked);
            buttons.Add(btn);
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
            var library = new string[nouns.Length + verbs.Length + adjectives.Length + prepositions.Length];
            Array.Copy(nouns, library, nouns.Length);
            int ix = nouns.Length;
            Array.Copy(verbs, 0, library, ix, verbs.Length);
            ix += verbs.Length;
            Array.Copy(adjectives, 0, library, ix, adjectives.Length);
            ix += adjectives.Length;
            Array.Copy(prepositions, 0, library, ix, prepositions.Length);

            var newSea = GenerateUniqueWords(wordSeaSize , library);
            return newSea;
        }
        if (currentLibraryName != "") {
            var library = PlayerPrefs.GetString(currentLibraryName);
            if (library == "")
                throw new InvalidOperationException("Empty library in WordSea.GenerateNewSea");
            return library.Split(';');
        }
        throw new InvalidOperationException("Empty currenLibrary in WordSea.GenerateNewSea");
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
