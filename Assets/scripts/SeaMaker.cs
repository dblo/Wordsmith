using System;
using UnityEngine;
using UnityEngine.UI;

public class SeaMaker : MonoBehaviour {
    private Text seaName;
    private Text seaContent;

    void Start () {
        seaName = GameObject.Find("SeaNameInputText").GetComponent<Text>();
        seaContent = GameObject.Find("SeaContentInputText").GetComponent<Text>();

        var saveButton = GameObject.Find("SeaMakerSaveButton").GetComponent<Button>();
        saveButton.onClick.AddListener(OnClickSaveButton);

        Action cancelBtnAction = () => {
            var parent = GameObject.Find("Canvas").transform;
            var go = (GameObject) Instantiate(Resources.Load("ConfirmationDialog"), parent);

            go.GetComponent<ConfirmationDialog>().SetOnConfirmAction(() => {
                Destroy(gameObject);
            });
        };
        var cancelButton = GameObject.Find("SeaMakerCancelButton").GetComponent<Button>();
        cancelButton.onClick.AddListener(delegate { cancelBtnAction(); });

        var newButton = GameObject.Find("SeaMakerNewButton").GetComponent<Button>();
        newButton.onClick.AddListener(OnClickNewButton);

        var clearPrefsButton = GameObject.Find("ClearPrefsButton").GetComponent<Button>();
        clearPrefsButton.onClick.AddListener(() => PlayerPrefs.DeleteAll());
    }

    public void OnClickSaveButton () {
        //Todo check not placehold text
        if (seaName.text != "" && seaContent.text != "") {
            var libraryNames = PlayerPrefs.GetString(PreferencesKeys.StoredLibraryNames);
            //Todo dont allow ; in seaName
            if (libraryNames == "") {
                PlayerPrefs.SetString(PreferencesKeys.StoredLibraryNames, seaName.text);
            } else {
                PlayerPrefs.SetString(PreferencesKeys.StoredLibraryNames, libraryNames + ";" + seaName.text);
            }
            PlayerPrefs.SetString(seaName.text, seaContent.text);
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
