using UnityEngine;
using UnityEngine.UI;

namespace OO {
    public class ScorePanel : MonoBehaviour {
        public Text ScoreTextPrefab;

        public void Start () {
            var gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            var scores = gm.GetScores();

            if(scores.Length < 3) {
                // Only show a single score for 2 players, since they always have the same score
                AddScore(0, 1, scores[0]);
                return;
            }
            for (int i = 0; i < scores.Length; i++) {
                AddScore((float) i / scores.Length, (float) (i + 1) / scores.Length, scores[i]);
            }
        }

        private void AddScore (float xMin, float xMax, int score) {
            var playerText = Instantiate(ScoreTextPrefab, transform);
            playerText.text = score.ToString();

            var anchorMin = new Vector2(xMin, 0);
            var anchorMax = new Vector2(xMax, 1);
            RectTransform rTrans = playerText.GetComponent<RectTransform>();
            rTrans.anchorMin = anchorMin;
            rTrans.anchorMax = anchorMax;
        }
    }
}
