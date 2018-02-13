using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OO {
    public class ScorePanel : MonoBehaviour {
       [SerializeField] private Text scoreTextPrefab;

        public void Setup(List<int> scores) {
            if(scores.Count< 3) {
                // Only show a single score for 2 players, since they always have the same score
                AddScore(0, 1, scores[0]);
                return;
            }
            for (var i = 0; i < scores.Count; i++) {
                AddScore((float) i / scores.Count, (float) (i + 1) / scores.Count, scores[i]);
            }
        }

        private void AddScore (float xMin, float xMax, int score) {
            var playerText = Instantiate(scoreTextPrefab, transform);
            playerText.text = score.ToString();

            var anchorMin = new Vector2(xMin, 0);
            var anchorMax = new Vector2(xMax, 1);
            var rTrans = playerText.GetComponent<RectTransform>();
            rTrans.anchorMin = anchorMin;
            rTrans.anchorMax = anchorMax;
        }
    }
}
