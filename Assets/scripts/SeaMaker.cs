using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OO {
    public class SeaMaker : MonoBehaviour {
        [SerializeField]
        private LibraryList libraryList;

        void Start () {
            var saveButton = GameObject.Find("SeaMakerSaveButton").GetComponent<Button>();
            saveButton.onClick.AddListener(OnClickSaveButton);

            Action cancelBtnAction = () => {
                var parent = GameObject.Find("Canvas").transform;
                var go = (GameObject) Instantiate(Resources.Load("ConfirmationDialog"), parent);

                go.GetComponent<ConfirmationDialog>().SetOnConfirmAction(() => {
                    SceneManager.LoadScene("main_menu");
                });
            };
            var cancelButton = GameObject.Find("SeaMakerCloseButton").GetComponent<Button>();
            cancelButton.onClick.AddListener(delegate { cancelBtnAction(); });

            var newButton = GameObject.Find("SeaMakerNewButton").GetComponent<Button>();
            newButton.onClick.AddListener(OnClickNewButton);

            var deleteButton = GameObject.Find("SeaMakerDeleteButton").GetComponent<Button>();
            deleteButton.onClick.AddListener(DeleteSelected);
        }

        private void DeleteSelected () {
            if (libraryList.SelectedListElement != null) {
                libraryList.DeleteSelected();
            }
        }

        public void OnClickSaveButton () {
            Text seaName = GameObject.Find("SeaNameInputText").GetComponent<Text>();
            Text seaContent = GameObject.Find("SeaContentInputText").GetComponent<Text>();

            //Todo check not placehold text
            if (seaName.text != "" && seaContent.text != "") {
                var libraryNames = PlayerPrefs.GetString(PreferencesKeys.CustomLibraryNames);
                //Todo dont allow ; in library names
                if (libraryNames == "") {
                    PlayerPrefs.SetString(PreferencesKeys.CustomLibraryNames, seaName.text + ";");
                } else {
                    PlayerPrefs.SetString(PreferencesKeys.CustomLibraryNames, libraryNames + seaName.text + ";");
                }
                var newLibraryJson = JsonArrayHelper.ToJson(seaContent.text.Split(' '));
                PlayerPrefs.SetString(seaName.text, newLibraryJson);
            }
        }

        public void OnClickNewButton () {
            var parent = GameObject.Find("Canvas").transform;
            var go = (GameObject) Instantiate(Resources.Load("ConfirmationDialog"), parent);
            go.GetComponent<ConfirmationDialog>().SetOnConfirmAction(() => {
                var contentInput = GameObject.Find("SeaContentInput").GetComponent<InputField>();
                contentInput.text = "";
                var nameInput = GameObject.Find("SeaNameInput").GetComponent<InputField>();
                nameInput.text = "";
            });
        }
    }
}
