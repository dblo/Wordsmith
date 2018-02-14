using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OO {
    public class ButtonBar : MonoBehaviour {
        [SerializeField] private Button goButton;
        [SerializeField] private Button homeButton;
        [SerializeField] private WordSea wordSea;
        [SerializeField] private Button saveLibraryButton;
        [SerializeField] private Text infoText;
        [SerializeField] private Text waitingText;
        [SerializeField] private GameObject playersJoiningText;
        private const float BUTTON_ANCHOR_X_WIDTH = 0.2f;
        private readonly List<Button> buttons = new List<Button>();
        private PlayerConnection localPlayer;

        public void OnGameOver () {
            waitingText.enabled = false;
            ToggleShowInfoText(false);
            goButton.gameObject.SetActive(false);
            homeButton.gameObject.SetActive(true);

            if (!localPlayer.isServer) {
                // Let any client save the used library by clicking this button
                saveLibraryButton.gameObject.SetActive(true);
            }
        }

        public bool TryAdd (Button btn) {
            ToggleShowInfoText(false);
            if (AllWordsChosen())
                return false;

            btn.GetComponent<WordButton>().MoveToButtonBar(transform, buttons.Count, OnClickWordButton);
            buttons.Add(btn);

            if (AllWordsChosen())
                goButton.interactable = true;
            return true;
        }

        public void SetLocalPlayer (PlayerConnection localPlayer) {
            this.localPlayer = localPlayer;
        }

        private void OnClickWordButton (Button btn) {
            MoveWordsIfNeeded(btn);
            buttons.Remove(btn);
            wordSea.ReturnWord(btn);
            goButton.interactable = false;
        }
        public void GameStarting () {
            playersJoiningText.SetActive(false);
            ToggleShowInfoText(true);
        }

        public void NewRound () {
            waitingText.enabled = false;
            ToggleShowInfoText(true);
        }

        private void ToggleShowInfoText (bool show) {
            infoText.enabled = show;
            if (show) {
                infoText.text = "Compose a line of " + GameData.Instance.GetLineLength() + " words.";
            }
        }

        private void ResetButtons () {
            foreach (var b in buttons) {
                wordSea.ReturnWord(b);
            }
            buttons.Clear();
        }

        public void OnClickGoButton () {
            localPlayer.WordsChosen(GetWords());
            ResetButtons();
            wordSea.SetSeaInteractable(false);
            waitingText.enabled = true;
            // Needed for case wgere the player has selected all words and then returns a word before pressing Go
            goButton.interactable = false;
        }

        public void OnClickHomeButton () {
            GameManager.LaunchMainMenu();
        }

        public void OnClickSaveLibraryButton () {
            saveLibraryButton.gameObject.SetActive(false);
            wordSea.SaveLibrary();
        }

        private string[] GetWords () {
            var words = new string[buttons.Count];
            for (var i = 0; i < buttons.Count; i++) {
                words[i] = buttons[i].GetComponentInChildren<Text>().text;
            }
            return words;
        }

        private bool AllWordsChosen () {
            return buttons.Count == GameData.Instance.GetLineLength();
        }

        private void MoveWordsIfNeeded (Button btn) {
            var rTrans = btn.GetComponent<RectTransform>();
            var minAnchorX = rTrans.anchorMin.x;
            var btnIndex = (int)(minAnchorX / BUTTON_ANCHOR_X_WIDTH);

            for (var i = buttons.Count - 1; i > btnIndex; i--) {
                var rTransRight = buttons[i].GetComponent<RectTransform>();
                var rTransLeft = buttons[i-1].GetComponent<RectTransform>();
                rTransRight.anchorMin = rTransLeft.anchorMin;
                rTransRight.anchorMax = rTransLeft.anchorMax;
            }
        }
    }
}