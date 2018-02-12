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
            for (int i = 0; i < GameData.Instance.LibraryCount; i++) {
                AddListElement(GameData.Instance.GetLibrary(i));
            }
        }

        public LibraryListButton GetSelectedLibrary () {
            if (selectedButton == null)
                return null;
            return selectedButton.GetComponentInChildren<LibraryListButton>();
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

        // May not be called if no element is selected
        public void DeleteSelected () {
            var index = selectedButton.transform.GetSiblingIndex();
            Button nextSelected = null;
            if (contentsTransform.childCount > index + 1) {
                // Element below exists
                nextSelected = contentsTransform.GetChild(index + 1).GetComponent<Button>();
            } else if (contentsTransform.childCount > 1) {
                // Element above exists
                nextSelected = contentsTransform.GetChild(index - 1).GetComponent<Button>();
            } else if (contentsTransform.childCount == 1) {
                // Leave nextSelected as null
            } else
                throw new InvalidOperationException("Did not account for this case");
            Destroy(selectedButton.gameObject);
            SetSelectedListElement(nextSelected);
        }
    }
}
