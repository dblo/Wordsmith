using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordButton : MonoBehaviour
{
    private WordSeaManager parent;

    void Start()
    {
        parent = GetComponentInParent<WordSeaManager>();
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => OnClick());
    }

    private void OnClick()
    {
        if(parent.WordClicked("foo"))
        {
            GetComponentInChildren<Text>().text = "";
        }
    }
}
