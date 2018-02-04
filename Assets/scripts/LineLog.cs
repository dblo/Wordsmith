using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OO {
    public class LineLog : MonoBehaviour {
        private Text playerLines;
        private Text playerLabel;
        private List<string> lines = new List<string>();
        private const float anchorYMin = 0f;
        private const float anchorYMax = 1f;

        public string PlayerName {
            get { return playerLabel.text; }
            set { playerLabel.text = value; }
        }

        public int LinesCount () {
            return lines.Count;
        }

        public string GetLinesAsString () {
            return playerLines.text;
        }

        private void Awake () {
            var gos = GetComponentsInChildren<Text>();
            foreach (var go in gos) {
                if (go.name == "Log")
                    playerLines = go;
                else if (go.name == "Label")
                    playerLabel = go;
            }
        }

        public void AddLine (string[] words, string[] colors) {
            var line = CreateRichText(words, colors);
            if (lines.Count > 0)
                playerLines.text = playerLines.text + "\n" + line.ToString();
            else
                playerLines.text = line.ToString();
            lines.Add(line);
        }

        public void AddTemporaryLine (string[] words, string[] colors) {
            var line = CreateRichText(words, colors);
            if (lines.Count > 0)
                playerLines.text = playerLines.text + "\n" + line.ToString();
            else
                playerLines.text = line.ToString();
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
            rTrans.anchorMin = new Vector2(anchorXMin, anchorYMin);
            rTrans.anchorMax = new Vector2(anchorXMax, anchorYMax);

            var ll = go.GetComponent<LineLog>();
            ll.PlayerName = playerName;
            return ll;
        }
    }
}
