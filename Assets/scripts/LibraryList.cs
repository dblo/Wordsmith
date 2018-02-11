using System;
using UnityEngine;
using UnityEngine.UI;

namespace OO {
    public class LibraryList : MonoBehaviour {
        public Button listElementPrefab;
        public Color SelectedColor;
        public Color NormalColor;

        private Transform contentsTransform;
        private Button selectedButton;
        // Callback set by externals
        private Action onSelectedAction;

        public bool HasSelection () {
            return selectedButton != null;
        }

        public void AddSelectedListener (Action action) {
            onSelectedAction = action;
        }

        private void Awake () {
            contentsTransform = GetComponentInChildren<VerticalLayoutGroup>().transform;
            SetupLibraries();
        }

        private void SetupLibraries () {
            foreach (var lib in GameData.Instance.GetLibraries()) {
                AddListElement(lib.name);
            }
        }

        public string GetSelectedText () {
            if (selectedButton == null)
                return null;
            return selectedButton.GetComponentInChildren<Text>().text;
        }

        // Add an element to contents and if its the first element then set it to be selected
        public void AddListElement (string name) {
            var go = Instantiate(listElementPrefab, contentsTransform);
            go.GetComponentInChildren<Text>().text = name;
            var btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => SetSelectedListElement(btn));

            if (selectedButton == null)
                SetSelectedListElement(btn);
        }

        private void SetSelectedListElement (Button btn) {
            if (selectedButton != null) {
                var colors = selectedButton.colors;
                colors.normalColor = NormalColor;
                selectedButton.colors = colors;
            }
            if (btn != null) {
                var colors = btn.colors;
                colors.normalColor = SelectedColor;
                colors.highlightedColor = SelectedColor;
                btn.colors = colors;
            }
            selectedButton = btn;

            if (onSelectedAction != null) {
                onSelectedAction();
            }
        }

        public void DeleteSelected () {
            // todo handle user cannot delete default libs? gray out button? handle return false below?
            var libraryName = selectedButton.GetComponentInChildren<Text>().text;
            GameData.Instance.DeleteLibrary(libraryName);

            Destroy(selectedButton.gameObject);
            SetSelectedListElement(null);
        }
    }
}
