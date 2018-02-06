using System;
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
            var defaultLibNames = PlayerPrefs.GetString(PreferencesKeys.DefaultLibraryNames, "")
                .Split(GC.LibraryNameDelimiter);
            if (defaultLibNames.Length == 0)
                throw new InvalidOperationException("No default libraries found in MultiplayerLobby.SetupLibraries()");

            foreach (var name in defaultLibNames) {
                AddListElement(name);
            }

            var customLibString = PlayerPrefs.GetString(PreferencesKeys.CustomLibraryNames, "");
            if (customLibString != "") {
                var customLibNames = customLibString.Split(GC.LibraryNameDelimiter);
                foreach (var name in customLibNames) {
                    if (name != "")
                        AddListElement(name);
                }
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
            var libraryName = SelectedListElement.GetComponentInChildren<Text>().text;
            PlayerPrefs.DeleteKey(libraryName);
            var customLibraryNames = PlayerPrefs.GetString(PreferencesKeys.CustomLibraryNames);
            customLibraryNames = customLibraryNames.Replace(libraryName + ";", "");
            PlayerPrefs.SetString(PreferencesKeys.CustomLibraryNames, customLibraryNames);

            Destroy(SelectedListElement.gameObject);
            SetSelectedListElement(null);
        }
    }
}
