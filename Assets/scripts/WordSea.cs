using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OO {
    public class WordSea : MonoBehaviour {
        public Button wordButtonPrefab;
        public ButtonBar buttonBar;
        public const int MaxCols = 4;
        public const int MaxRows = 3;

        private List<Button> buttons;
        private List<string> currentSea;

        private void Awake () {
            buttons = new List<Button>(GameData.Instance.GetSeaSize());
            currentSea = new List<string>(GameData.Instance.GetSeaSize());
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
            for (int i = 0; i < GameData.Instance.GetSeaSize(); i++) {
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
            return GenerateUniqueWords(GameData.Instance.GetSeaSize(), 
                GameData.Instance.GetSelectedLibrary().words);
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
}
