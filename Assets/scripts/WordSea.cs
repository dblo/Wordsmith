using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OO {
    public class WordSea : MonoBehaviour {
        public Button wordButtonPrefab;
        public ButtonBar buttonBar;
        public const int MAX_COLS = 4;
        public const int MAX_ROWS = 3;

        private List<Button> buttons = new List<Button>(MAX_COLS * MAX_ROWS);
        private static System.Random rng = new System.Random();

        public void ReturnWord (Button btn) {
            btn.GetComponent<WordButton>().MoveToWordSea(transform, WordClicked);
        }

        private void WordClicked (Button btn) {
            if (buttonBar.TryAdd(btn)) {
                btn.GetComponent<WordButton>().PlayWordChosenSounds();
            }
        }

        public void SetSeaInteractable (bool value) {
            foreach (var b in buttons) {
                b.interactable = value;
            }
        }

        private void CreateButtons (int seaSize) {
            for (int i = 0; i < seaSize; i++) {
                var btn = Instantiate(wordButtonPrefab, transform);
                var wb = btn.GetComponent<WordButton>();
                wb.ComputeSeaAnchors(i);
                wb.MoveToWordSea(transform, WordClicked);
                buttons.Add(btn);
            }
        }

        public void SetNewSea (string[] words) {
            if(buttons.Count != words.Length) {
                buttons.ForEach(b => Destroy(b.gameObject));
                buttons.Clear();
                CreateButtons(words.Length);
            }
            for (int i = 0; i < words.Length; i++) {
                var text = buttons[i].GetComponentInChildren<Text>();
                text.text = words[i];
            }
            SetSeaInteractable(true);
        }

        public static string[] GenerateNewSea (string[] libraryWords, int seaSize) {
            Debug.Assert(libraryWords.Length >= seaSize);

            int libLen = libraryWords.Length;
            var indices = new HashSet<int>();
            var seaWords = new string[seaSize];

            for (int i = 0; i < seaSize; i++) {
                int r = rng.Next(libLen);
                while (indices.Contains(r)) {
                    r = rng.Next(libLen);
                }
                indices.Add(r);
                seaWords[i] = libraryWords[r];
            }
            return seaWords;
        }
    }
}
