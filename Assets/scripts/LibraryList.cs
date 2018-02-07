using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OO {
    public class LibraryList : MonoBehaviour {
        public Button listElementPrefab;
        public Transform contentsTransform;
        private readonly Color SelectedColor = new Color (118 / 255f, 184 / 255f, 206 / 255f, 1f);

        public Button SelectedListElement { get; private set; }

        void Start () {
            SetupLibraries();
        }

        private void SetupLibraries () {
            foreach (var lib in GameData.Instance.GetLibraries()) {
                AddListElement(lib.name);
            }
        }

        public void AddListElement (string name) {
            var go = Instantiate(listElementPrefab, contentsTransform);
            go.GetComponentInChildren<Text>().text = name;
            var btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => SetSelectedListElement(btn));
        }

        private void SetSelectedListElement (Button btn) {
            if (SelectedListElement != null) {
                var colors = SelectedListElement.colors;
                colors.normalColor = Color.white;
                SelectedListElement.colors = colors;
            }
            if (btn != null) {
                var colors = btn.colors;
                colors.normalColor = SelectedColor;
                colors.highlightedColor = colors.normalColor;
                btn.colors = colors;
            }
            SelectedListElement = btn;
        }

        internal void DeleteSelected () {
            // todo handle user cannot delete default libs? gray out button? handle return false below?
            var libraryName = SelectedListElement.GetComponentInChildren<Text>().text;
            GameData.Instance.DeleteLibrary(libraryName);

            Destroy(SelectedListElement.gameObject);
            SetSelectedListElement(null);
        }
    }
}
