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
            var listElement = contentsTransform.GetComponentInChildren<Button>();
            if (listElement != null) {
                // The list has at least one element in the scene, set first as selected
                SetSelectedListElement(listElement);
            }
        }

        private void Start () {
            SetupLibraries();
        }

        private void SetupLibraries () {
            foreach (var lib in GameData.Instance.GetLibraries()) {
                AddListElement(lib);
            }
        }

        public Library GetSelectedLibrary () {
            if (selectedButton == null)
                return null;
            return selectedButton.GetComponentInChildren<LibraryListButton>().Library;
        }

        // Add an element to contents and if its the first element then set it to be selected
        public void AddListElement (Library library) {
            var go = Instantiate(listElementPrefab, contentsTransform);
            go.GetComponent<LibraryListButton>().Setup(library);

            var btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => SetSelectedListElement(btn));

            if (selectedButton == null)
                SetSelectedListElement(btn);
        }

        public void SetSelectedListElement (Button btn) {
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

            var index = selectedButton.transform.GetSiblingIndex();
            Button nextSelected = null;
            if (contentsTransform.childCount > index + 1)
                nextSelected = contentsTransform.GetChild(index + 1).GetComponent<Button>();
            else if (contentsTransform.childCount > 1 && index == contentsTransform.childCount - 1)
                nextSelected = contentsTransform.GetChild(index - 1).GetComponent<Button>();
            else if (contentsTransform.childCount > 1)
                nextSelected = contentsTransform.GetChild(0).GetComponent<Button>();
            // else deleting last element, do nothing

            Destroy(selectedButton.gameObject);
            SetSelectedListElement(nextSelected);
        }
    }
}
