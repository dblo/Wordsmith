using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OO {
    public class SeaMaker : MonoBehaviour {
        [SerializeField] private LibraryList libraryList;
        private InputField seaName;
        private InputField seaContent;

        private void Start () {
            seaName = GameObject.Find("SeaNameInput").GetComponent<InputField>();
            seaContent = GameObject.Find("SeaContentInput").GetComponent<InputField>();

            var saveButton = GameObject.Find("SeaMakerSaveButton").GetComponent<Button>();
            saveButton.onClick.AddListener(OnClickSaveButton);

            var cancelButton = GameObject.Find("CloseButton").GetComponent<Button>();
            cancelButton.onClick.AddListener(OnClickCloseButton);

            var newButton = GameObject.Find("SeaMakerNewButton").GetComponent<Button>();
            newButton.onClick.AddListener(OnClickNewButton);

            var deleteButton = GameObject.Find("SeaMakerDeleteButton").GetComponent<Button>();
            deleteButton.onClick.AddListener(DeleteSelected);

            libraryList.AddSelectedListener(OnLibrarySelected);
        }

        private void DeleteSelected () {
            if (!libraryList.HasSelection()) return;

            Action delete = () => {
                var selectedLibrary = libraryList.GetSelectedLibrary().Library;
                GameData.Instance.DeleteLibrary(selectedLibrary);
                libraryList.DeleteSelected();
            };
            var msg = "Really delete " + libraryList.GetSelectedLibrary().Library.Name + "?";
            ConfirmationDialog.Create(msg, delete, transform.parent);
        }

        public void OnClickCloseButton () {
            ConfirmationDialog.Create("Discard unsaved changes?",
                () => SceneManager.LoadScene("main_menu"), transform.parent);
        }

        public void OnClickSaveButton () {
            if (seaName.text == "" || seaContent.text == "") {
                return;
            }

            var choices = new SeaContent(seaContent.text.Split(' '));
            var library = new Library(seaName.text, true, new List<SeaContent>() { choices });
            var existingLibrary = GameData.Instance.FindLibrary(library.Name);
            if (existingLibrary == null) {
                GameData.Instance.AddLibrary(library);
                libraryList.AddListElement(library);
                return;
            }
            Action overwrite = () => {
                GameData.Instance.ReplaceLibrary(library);
                libraryList.GetSelectedLibrary().Library = library;
            };
            var msg = existingLibrary.PlayerMade ? "Overwrite?" : "Overwrite DEFAULT library?";
            ConfirmationDialog.Create(msg, overwrite, transform.parent);
        }

        public void OnClickNewButton () {
            Action discard = () => {
                var contentInput = transform.Find("SeaContentInput").GetComponent<InputField>();
                contentInput.text = "";
                var nameInput = transform.Find("SeaNameInput").GetComponent<InputField>();
                nameInput.text = "";
            };
            ConfirmationDialog.Create("Discard unsaved changes?", discard, transform.parent);
        }

        private void OnLibrarySelected () {
            if (libraryList.HasSelection()) {
                var library = libraryList.GetSelectedLibrary().Library;
                seaName.text = library.Name;
                seaContent.text = string.Join(" ", library.GetChoices(0));
            } else {
                seaName.text = "";
                seaContent.text = "";
            }
        }
    }
}
