using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OO {
    public class SeaMaker : MonoBehaviour {
        public LibraryList libraryList;
        private InputField seaName;
        private InputField seaContent;

        void Start () {
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
            if (libraryList.HasSelection()) {
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
            if (seaName.text == "" || seaContent.text == "")
                return; //todo tell use

            var library = new Library() { name = seaName.text, playerMade = true, words = seaContent.text.Split(' ') };
            if (GameData.Instance.AddLibrary(library)) {
                libraryList.AddListElement(library);
            }
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

        public void OnLibrarySelected () {
            if (libraryList.HasSelection()) {
                var library = libraryList.GetSelectedLibrary();
                seaName.text = library.name;
                seaContent.text = string.Join(" ", library.words);
            } else {
                seaName.text = "";
                seaContent.text = "";
            }
        }
    }
}
