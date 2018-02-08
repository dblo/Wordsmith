using UnityEngine;
using UnityEngine.UI;

namespace OO {
    public class LibraryList : MonoBehaviour {
        public Button listElementPrefab;
        public Color SelectedColor;
        public Color NormalColor;

        private Transform contentsTransform;

        public Button SelectedListElement { get; private set; }

        void Start () {
            contentsTransform = GetComponentInChildren<VerticalLayoutGroup>().transform;
            SetupLibraries();
        }

        private void SetupLibraries () {
            foreach (var lib in GameData.Instance.GetLibraries()) {
                AddListElement(lib.name);
            }
        }

        // Add an element to contents and if its the first element then set it to be selected
        public void AddListElement (string name) {
            var go = Instantiate(listElementPrefab, contentsTransform);
            go.GetComponentInChildren<Text>().text = name;
            var btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => SetSelectedListElement(btn));

            if (SelectedListElement == null)
                SetSelectedListElement(btn);
        }

        private void SetSelectedListElement (Button btn) {
            if (SelectedListElement != null) {
                var colors = SelectedListElement.colors;
                colors.normalColor = NormalColor;
                SelectedListElement.colors = colors;
            }
            if (btn != null) {
                var colors = btn.colors;
                colors.normalColor = SelectedColor;
                colors.highlightedColor = SelectedColor;
                btn.colors = colors;
            }
            SelectedListElement = btn;
        }

        public void DeleteSelected () {
            // todo handle user cannot delete default libs? gray out button? handle return false below?
            var libraryName = SelectedListElement.GetComponentInChildren<Text>().text;
            GameData.Instance.DeleteLibrary(libraryName);

            Destroy(SelectedListElement.gameObject);
            SetSelectedListElement(null);
        }
    }
}
