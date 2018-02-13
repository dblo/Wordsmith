using System;
using UnityEngine;
using UnityEngine.UI;

namespace OO {
    public class LibraryList : MonoBehaviour {
        [SerializeField] private Button listElementPrefab;
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color normalColor;
        [SerializeField] private Transform content;

        private Button selectedElement;
        private Action onSelectedAction; // Callback set by externals

        public bool HasSelection () {
            return selectedElement != null;
        }

        public void AddSelectedListener (Action action) {
            onSelectedAction = action;
        }

        private void Awake () {
            var listElement = content.GetComponentInChildren<Button>();
            if (listElement != null) {
                // The list has at least one element in the scene, set first as selected
                SetSelectedListElement(listElement);
            }
            SetupLibraries();
        }

        private void SetupLibraries () {
            for (int i = 0; i < GameData.Instance.LibraryCount; i++) {
                AddListElement(GameData.Instance.GetLibrary(i));
            }
        }

        public LibraryListButton GetSelectedLibrary () {
            if (selectedElement == null)
                return null;
            return selectedElement.GetComponentInChildren<LibraryListButton>();
        }

        // Add an element to contents and if its the first element then set it to be selected
        public void AddListElement (Library library) {
            var go = Instantiate(listElementPrefab, content);
            go.GetComponent<LibraryListButton>().Setup(library);

            var btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => SetSelectedListElement(btn));

            if (selectedElement == null)
                SetSelectedListElement(btn);
        }

        private void SetSelectedListElement (Button btn) {
            if (selectedElement != null) {
                var colors = selectedElement.colors;
                colors.normalColor = normalColor;
                selectedElement.colors = colors;
            }
            if (btn != null) {
                var colors = btn.colors;
                colors.normalColor = selectedColor;
                colors.highlightedColor = selectedColor;
                btn.colors = colors;
            }
            selectedElement = btn;

            if (onSelectedAction != null) {
                onSelectedAction();
            }
        }

        // May not be called if no element is selected
        public void DeleteSelected () {
            var index = selectedElement.transform.GetSiblingIndex();
            Button nextSelected = null;
            if (content.childCount > index + 1) {
                // Element below exists
                nextSelected = content.GetChild(index + 1).GetComponent<Button>();
            } else if (content.childCount > 1) {
                // Element above exists
                nextSelected = content.GetChild(index - 1).GetComponent<Button>();
            } else if (content.childCount == 1) {
                // Leave nextSelected as null
            } else
                throw new InvalidOperationException("Did not account for this case");
            Destroy(selectedElement.gameObject);
            SetSelectedListElement(nextSelected);
        }
    }
}
