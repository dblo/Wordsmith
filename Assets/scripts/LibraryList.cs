using System;
using UnityEngine;
using UnityEngine.UI;

namespace OO {
    public class LibraryList : MonoBehaviour {
        public Button listElementPrefab;
        public Transform contentsTransform;
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
                    AddListElement(name);
                }
            }
        }

        private void AddListElement (string name) {
            var go = Instantiate(listElementPrefab, contentsTransform);
            go.GetComponentInChildren<Text>().text = name;
            var btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => WordSeaListButtonClicked(btn));
        }

        private void WordSeaListButtonClicked (Button btn) {
            SelectedListElement = btn;
        }

        internal void DeleteSelected () {
            var libraryName = SelectedListElement.GetComponentInChildren<Text>().text;
            PlayerPrefs.DeleteKey(libraryName);
            var customLibraryNames = PlayerPrefs.GetString(PreferencesKeys.CustomLibraryNames);
            customLibraryNames  = customLibraryNames.Replace(libraryName + ";", "");
            PlayerPrefs.SetString(PreferencesKeys.CustomLibraryNames, customLibraryNames);

            Destroy(SelectedListElement.gameObject);
            SelectedListElement = null;
        }
    }
}
