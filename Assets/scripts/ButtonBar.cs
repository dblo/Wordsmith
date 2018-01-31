using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBar : MonoBehaviour {
    public static int lineLength = 4;
    public Button goButton;
    public WordSea wordSea;

    private List<Button> buttons = new List<Button>();
    private PlayerConnection localPlayer;
    private Text waitingText;

    private void Awake () {
        waitingText = GameObject.Find("WaitingText").GetComponent<Text>();
    }

    // Return the number of buttons in the buttonBar incluing wordButtons and the Go button.
    public static int ButtonCols { get { return lineLength + 1; } }

    public void OnGameOver () {
        waitingText.enabled = false;
    }

    public bool TryAdd (Button btn) {
        if (AllWordsChosen())
            return false;

        btn.GetComponent<WordButton>().MoveToButtonBar(transform, buttons.Count, WordClicked);
        buttons.Add(btn);

        if (AllWordsChosen())
            goButton.interactable = true;
        return true;
    }

    public void AssignLocalPlayer (PlayerConnection localPlayer) {
        this.localPlayer = localPlayer;
    }

    public void WordClicked (Button btn) {
        ShiftWordsLeft(btn);
        buttons.Remove(btn);
        wordSea.ReturnWord(btn);
        goButton.interactable = false;
    }

    public void Reset () {
        foreach (var b in buttons) {
            wordSea.ReturnWord(b);
        }
        buttons.Clear();
        waitingText.enabled = false;
    }

    public void AcceptLine () {
        localPlayer.WordsChosen(GetWords());
        goButton.interactable = false;
        waitingText.enabled = true;
    }

    private string[] GetWords () {
        string[] words = new string[buttons.Count];
        for (int i = 0; i < buttons.Count; i++) {
            words[i] = buttons[i].GetComponentInChildren<Text>().text;
        }
        return words;
    }

    private bool AllWordsChosen () {
        return buttons.Count == lineLength;
    }

    private void ShiftWordsLeft (Button btn) {
        var rTrans = btn.GetComponent<RectTransform>();
        var minAnchorX = rTrans.anchorMin.x;
        int btnIndex = (int)(minAnchorX / 0.2f);

        for (int i = buttons.Count - 1; i > btnIndex; i--) {
            var rTransRight = buttons[i].GetComponent<RectTransform>();
            var rTransLeft = buttons[i-1].GetComponent<RectTransform>();
            rTransRight.anchorMin = rTransLeft.anchorMin;
            rTransRight.anchorMax = rTransLeft.anchorMax;
        }
    }
}
