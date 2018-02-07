using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OO {
    public class SeaMaker : MonoBehaviour {
        public LibraryList libraryList;
        private Text seaName;
        private Text seaContent;

        void Start () {
            seaName = GameObject.Find("SeaNameInputText").GetComponent<Text>();
            seaContent = GameObject.Find("SeaContentInputText").GetComponent<Text>();

            var saveButton = GameObject.Find("SeaMakerSaveButton").GetComponent<Button>();
            saveButton.onClick.AddListener(OnClickSaveButton);

            var cancelButton = GameObject.Find("SeaMakerCloseButton").GetComponent<Button>();
            cancelButton.onClick.AddListener(OnClickCloseButton);

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

        public void OnClickCloseButton () {
            if (NoChangesPending()) {
                SceneManager.LoadScene("main_menu");
                return;
            }
            var go = (GameObject) Instantiate(Resources.Load("ConfirmationDialog"), transform.parent);
            go.GetComponent<ConfirmationDialog>().SetOnConfirmAction(() => {
                SceneManager.LoadScene("main_menu");
            });
        }

        private bool NoChangesPending () {
            return seaName.text == "" && seaContent.text == "";
        }

        public void OnClickSaveButton () {
            if (seaName.text != "" && seaContent.text != "") {
                var library = new Library() { name = seaName.text, playerMade = true, words = seaContent.text.Split(' ') };
                GameData.Instance.AddLibrary(library);
            }
            libraryList.AddListElement(seaName.text);
        }

        public void OnClickNewButton () {
            var go = (GameObject) Instantiate(Resources.Load("ConfirmationDialog"), transform.parent);
            go.GetComponent<ConfirmationDialog>().SetOnConfirmAction(() => {
                var contentInput = transform.Find("SeaContentInput").GetComponent<InputField>();
                contentInput.text = "";
                var nameInput = transform.Find("SeaNameInput").GetComponent<InputField>();
                nameInput.text = "";
            });
        }
    }
}
