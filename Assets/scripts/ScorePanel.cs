using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OO {
    public class ScorePanel : MonoBehaviour {
       [SerializeField] private Text scoreTextPrefab;

        public void Setup(int score, int maxScore) {
            var scoreText = score + " / " + maxScore;
            AddScore(0, 1, scoreText);
        }

        private void AddScore (float xMin, float xMax, string text) {
            var playerText = Instantiate(scoreTextPrefab, transform);
            playerText.text = text;

            var anchorMin = new Vector2(xMin, 0);
            var anchorMax = new Vector2(xMax, 1);
            var rTrans = playerText.GetComponent<RectTransform>();
            rTrans.anchorMin = anchorMin;
            rTrans.anchorMax = anchorMax;
        }
    }
}
