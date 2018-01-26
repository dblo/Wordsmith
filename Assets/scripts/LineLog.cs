using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineLog : MonoBehaviour {
    private Text plyerLines;
    private Text playerLabel;
    private List<string> lines = new List<string>();
    private const float anchorYMin = 0.65f;
    private const float anchorYMax = 1f;

    public string PlayerName {
        get { return playerLabel.text; }
        set { playerLabel.text = value; }
    }

    public int LinesCount () {
        return lines.Count;
    }

    public string GetLinesAsString () {
        return plyerLines.text;
    }

    private void Awake () {
        var gos = GetComponentsInChildren<Text>();
        foreach (var go in gos) {
            if (go.name == "Log")
                plyerLines = go;
            else if (go.name == "Label")
                playerLabel = go;
        }
    }

    public void AddLine (string[] words, string[] colors) {
        var line = CreateRichText(words, colors);
        if (lines.Count > 0)
            plyerLines.text = plyerLines.text + "\n" + line.ToString();
        else
            plyerLines.text = line.ToString();
        lines.Add(line);
    }

    private static string CreateRichText(string[] words, string[]colors) {
        string str = "";
        for (int i = 0; i < words.Length - 1; i++)
            str += "<color=" + colors[i] + ">" + words[i] + "</color>" + " ";
        str += "<color=" + colors[words.Length - 1] + ">" + words[words.Length - 1] + "</color>";
        return str;
    }

    // Create an instance of the LineLog prefab in the Resources the folder
    public static LineLog Create (float anchorXMin, float anchorXMax, string playerName) {
        var canvasTrans = GameObject.Find("Canvas").transform;
        var go = Instantiate((GameObject) Resources.Load("LineLog"), canvasTrans, false);
        var rTrans = go.GetComponent<RectTransform>();
        rTrans.anchorMin = new Vector2(anchorXMin, anchorYMin);
        rTrans.anchorMax = new Vector2(anchorXMax, anchorYMax);

        var ll = go.GetComponent<LineLog>();
        ll.PlayerName = playerName;
        return ll;
    }
}
