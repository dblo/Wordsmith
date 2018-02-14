using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OO {
    public class LineLog : MonoBehaviour {
        [SerializeField] private Text playerLines;
        [SerializeField] private Text playerLabel;
        private readonly List<string> lines = new List<string>();
        private const float ANCHOR_Y_MIN = 0f;
        private const float ANCHOR_Y_MAX = 1f;

        public void AddLine (string[] words, string[] colors) {
            var line = CreateRichText(words, colors);
            if (lines.Count > 0)
                playerLines.text = playerLines.text + "\n" + line;
            else
                playerLines.text = line;
            lines.Add(line);
        }

        public void AddTemporaryLine (string[] words, string[] colors) {
            var line = CreateRichText(words, colors);
            if (lines.Count > 0)
                playerLines.text = playerLines.text + "\n" + line;
            else
                playerLines.text = line;
        }

        public void RemoveTemporaryLine () {
            var ix = playerLines.text.LastIndexOf('\n');
            if (ix < 0)
                playerLines.text = "";
            else
                playerLines.text = playerLines.text.Remove(ix);
        }

        private static string CreateRichText (string[] words, string[] colors) {
            string str = "";
            for (int i = 0; i < words.Length - 1; i++)
                str += "<color=" + colors[i] + ">" + words[i] + "</color>" + " ";
            str += "<color=" + colors[words.Length - 1] + ">" + words[words.Length - 1] + "</color>";
            return str;
        }

        // Create an instance of the LineLog prefab in the Resources the folder
        public static LineLog Create (float anchorXMin, float anchorXMax, string playerName) {
            var parent = GameObject.Find("LineLogs").transform;
            var go = Instantiate((GameObject) Resources.Load("LineLog"), parent, false);
            var rTrans = go.GetComponent<RectTransform>();
            rTrans.anchorMin = new Vector2(anchorXMin, ANCHOR_Y_MIN);
            rTrans.anchorMax = new Vector2(anchorXMax, ANCHOR_Y_MAX);

            var ll = go.GetComponent<LineLog>();
            ll.playerLabel.text = playerName;
            return ll;
        }
    }
}
