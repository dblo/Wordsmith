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

        var cancelButton = GameObject.Find("SeaMakerCancelButton").GetComponent<Button>();
        cancelButton.onClick.AddListener(() => Destroy(gameObject));

        var newButton = GameObject.Find("SeaMakerNewButton").GetComponent<Button>();
        newButton.onClick.AddListener(OnClickNewButton);

        var clearPrefsButton = GameObject.Find("ClearPrefsButton").GetComponent<Button>();
        clearPrefsButton.onClick.AddListener(() => PlayerPrefs.DeleteAll());
    }

    public void OnClickSaveButton () {
        //Todo check not placehold text
        if(seaName.text != "" && seaContent.text != "") {
            var libraryNames = PlayerPrefs.GetString(WordSea.PP_LIBRARY_NAMES);
            //Todo dont allow ; in seaName
            if(libraryNames == "") {
                PlayerPrefs.SetString(WordSea.PP_LIBRARY_NAMES, seaName.text);
            } else {
                PlayerPrefs.SetString(WordSea.PP_LIBRARY_NAMES, libraryNames + ";" + seaName.text);
            }
            PlayerPrefs.SetString(seaName.text, seaContent.text);
        }
    }

    public void OnClickNewButton() {
        var contentInput = GameObject.Find("SeaContentInput").GetComponent<InputField>();
        contentInput.text = "";

        var nameInput = GameObject.Find("SeaNameInput").GetComponent<InputField>();
        nameInput.text = "";
    }
}
