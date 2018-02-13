using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OO {
    public class WordSea : MonoBehaviour {
        public Button wordButtonPrefab;
        public ButtonBar buttonBar;
        public const int MAX_COLS = 4;
        public const int MAX_ROWS = 3;

        private Library currentLibrary;
        private List<Button> buttons;
        private static System.Random rng = new System.Random();

        public void ReturnWord (Button btn) {
            btn.GetComponent<WordButton>().MoveToWordSea(transform, WordClicked);
        }

        private void WordClicked (Button btn) {
            if (buttonBar.TryAdd(btn)) {
                btn.GetComponent<WordButton>().PlayChoseWordSounds();
            }
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
            for (int i = 0; i < words.Length; i++) {
                var text = buttons[i].GetComponentInChildren<Text>();
                text.text = words[i];
            }
            SetSeaFrozen(false);
        }

        public static string[] GenerateNewSea (Library library, int seaSize) {
            Debug.Assert(library.Words.Length >= seaSize);

            int libLen = library.Words.Length;
            var indices = new HashSet<int>();
            var words = new string[seaSize];

            for (int i = 0; i < seaSize; i++) {
                int r = rng.Next(libLen);
                while (indices.Contains(r)) {
                    r = rng.Next(libLen);
                }
                indices.Add(r);
                words[i] = library.Words[r];
            }
            return words;
        }
    }
}
