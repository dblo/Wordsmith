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

        private Library currentLibrary;
        private List<Button> buttons;
        private List<string> currentSea;
        private static System.Random rng = new System.Random();

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

        public void UseNewLibrary (Library library) {
            currentLibrary = library;
            int seaSize = GameData.Instance.GetSeaSize();

            buttons = new List<Button>(seaSize);
            currentSea = new List<string>(seaSize);
     
            for (int i = 0; i < seaSize; i++) {
                var btn = Instantiate(wordButtonPrefab, transform);
                var wb = btn.GetComponent<WordButton>();
                wb.ComputeSeaAnchors(i);
                wb.MoveToWordSea(transform, WordClicked);
                buttons.Add(btn);
            }
        }

        public void SaveLibrary(){
            GameData.Instance.AddLibrary(currentLibrary);
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

        public string[] GenerateNewSea()
        {
            return GenerateNewSea(currentLibrary, GameData.Instance.GetSeaSize());
        }

        public static string[] GenerateNewSea (Library library, int seaSize) {
            Debug.Assert(library.words.Length >= seaSize);

            int libLen = library.words.Length;
            var indices = new HashSet<int>();
            var words = new string[seaSize];

            for (int i = 0; i < seaSize; i++) {
                int r = rng.Next(libLen);
                while (indices.Contains(r)) {
                    r = rng.Next(libLen);
                }
                indices.Add(r);
                words[i] = library.words[r];
            }
            return words;
        }
    }
}
