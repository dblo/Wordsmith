﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OO {
    public class ButtonBar : MonoBehaviour {
        public Button goButton;
        public WordSea wordSea;
        public Button saveLibraryButton;
        public Text infoText;

        private List<Button> buttons = new List<Button>();
        private PlayerConnection localPlayer;
        private Text waitingText;
        private GameObject playersJoiningText;

        private void Awake () {
            waitingText = GameObject.Find("WaitingText").GetComponent<Text>();
            playersJoiningText = GameObject.Find("PlayerJoiningText");
            goButton.onClick.AddListener(OnGoButtonClicked);
        }

        public void OnGameOver () {
            OnNewRound();
            goButton.interactable = true;
            goButton.GetComponentInChildren<Text>().text = "Home";
            goButton.onClick.RemoveAllListeners();
            goButton.onClick.AddListener(OnHomeButtonClicked);
            ToggleShowInfoText(false);

            if (!localPlayer.isServer) {
                saveLibraryButton.gameObject.SetActive(true);
            }
        }

        public bool TryAdd (Button btn) {
            ToggleShowInfoText(false);
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
        public void GameStarting () {
            playersJoiningText.SetActive(false);
            ToggleShowInfoText(true);
        }

        public void OnNewRound () {
            waitingText.enabled = false;
            ToggleShowInfoText(true);
        }

        private void ToggleShowInfoText (bool show) {
            infoText.enabled = show;
            if (show) {
                infoText.text = "Compose a line of " + GameData.Instance.GetLineLength() + " words.";
            }
        }

        private void Reset () {
            foreach (var b in buttons) {
                wordSea.ReturnWord(b);
            }
            buttons.Clear();
        }

        public void OnGoButtonClicked () {
            localPlayer.WordsChosen(GetWords());
            Reset();
            wordSea.SetSeaFrozen(true);
            goButton.interactable = false;
            waitingText.enabled = true;
        }

        public void OnHomeButtonClicked () {
            var gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            gm.LaunchMainMenu();
        }

        public void OnSaveLibrary () {
            saveLibraryButton.gameObject.SetActive(false);
            wordSea.SaveLibrary();
        }

        private string[] GetWords () {
            string[] words = new string[buttons.Count];
            for (int i = 0; i < buttons.Count; i++) {
                words[i] = buttons[i].GetComponentInChildren<Text>().text;
            }
            return words;
        }

        private bool AllWordsChosen () {
            return buttons.Count == GameData.Instance.GetLineLength();
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
}