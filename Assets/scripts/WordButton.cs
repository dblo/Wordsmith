using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordButton : MonoBehaviour
{
    private WordSeaManager wordSea;
    private Text text;
    private Button button;

    void Start()
    {
        wordSea = GetComponentInParent<WordSeaManager>();
        button = GetComponent<Button>();
        text = button.GetComponentInChildren<Text>();
        button.onClick.AddListener(() => OnClick());
    }

    private void OnClick()
    {
        if(wordSea.WordClicked(text.text))
        {
            text.text = "";
            button.interactable = false;
        }
    }
}
