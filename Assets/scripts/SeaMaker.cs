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
                LibraryManager.Instance.DeleteLibrary(selectedLibrary);
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

            List<SeaContent> seaChoices = new List<SeaContent>();
            if (seaContent.text.Contains("\n")) {
                var roundContent = seaContent.text.Split('\n');
                foreach (var row in roundContent) {
                    seaChoices.Add(new SeaContent(row.Split(' ')));
                }
            } else {
                SeaContent choices = new SeaContent(seaContent.text.Split(' '));
                seaChoices.Add(choices);
            }

            var library = new Library(seaName.text, true, seaChoices);
            var existingLibrary = LibraryManager.Instance.FindLibrary(library.Name);
            if (existingLibrary == null) {
                LibraryManager.Instance.AddLibrary(library);
                libraryList.AddListElement(library);
                return;
            }
            Action overwrite = () => {
                LibraryManager.Instance.ReplaceLibrary(library);
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
                seaContent.text = library.GetAllChoices();
            } else {
                seaName.text = "";
                seaContent.text = "";
            }
        }
    }
}
