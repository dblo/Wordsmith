using System;
using UnityEngine;
using UnityEngine.UI;

namespace OO {
    public class WordButton : MonoBehaviour {
        private Vector2 minSeaAnchors;
        private Vector2 maxSeaAnchors;
        private AudioSource[] audioSorces;
        private Button button;
        private RectTransform rectTrans;

        private void Awake () {
            audioSorces = GetComponents<AudioSource>();
            button = GetComponent<Button>();
            rectTrans = GetComponent<RectTransform>();
        }

        public void ComputeSeaAnchors (int index) {
            float x = index % WordSea.MAX_COLS;
            float y = index / WordSea.MAX_COLS;
            minSeaAnchors = new Vector2(x / WordSea.MAX_COLS, y / WordSea.MAX_ROWS);
            maxSeaAnchors = new Vector2((x + 1) / WordSea.MAX_COLS, (y + 1) / WordSea.MAX_ROWS);
        }

        public void MoveToWordSea (Transform parent, Action<Button> onClickCallback) {
            button.transform.SetParent(parent, false);
            rectTrans.anchorMin = minSeaAnchors;
            rectTrans.anchorMax = maxSeaAnchors;

            if (!AudioManager.Instance.SoundMuted) {
                audioSorces[1].Play();
            }
            SetListener(onClickCallback);
        }

        public void MoveToButtonBar (Transform parent, float xIndex, Action<Button> onClickAction) {
            button.transform.SetParent(parent, false);

            float xMin;
            float xMax;
            switch (GameData.Instance.GetLineLength()) {
                case 1:
                    xMin = 0.3f;
                    xMax = 0.5f;
                    break;
                case 2:
                    xMin = 0.2f * (xIndex + 1);
                    xMax = 0.2f * (xIndex + 2);
                    break;
                case 3:
                    xMin = 0.1f + 0.2f * xIndex;
                    xMax = 0.1f + 0.2f * (xIndex + 1);
                    break;
                case 4:
                    xMin = 0.2f * xIndex;
                    xMax = 0.2f * (xIndex + 1);
                    break;
                default:
                    throw new InvalidOperationException("Unhandled ButtonBar.lineLength");
            }
            var anchorMin = new Vector2(xMin, 0);
            var anchorMax = new Vector2(xMax, 1);
            rectTrans.anchorMin = anchorMin;
            rectTrans.anchorMax = anchorMax;
            SetListener(onClickAction);
        }

        public void PlayWordChosenSounds () {
            if (!AudioManager.Instance.SoundMuted) {
                audioSorces[0].Play();
            }
        }

        private void SetListener (Action<Button> action) {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate { action(button); });
        }
    }
}
