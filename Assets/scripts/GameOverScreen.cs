using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class GameOverScreen : MonoBehaviour
{
    private Text p1LinesText;
    private Text p2LinesText;
    private Text scorePerLineText;
    private Text scoreTotalText;

    private void Awake()
    {
        p1LinesText = GameObject.Find("Player1Lines").GetComponent<Text>();
        p2LinesText = GameObject.Find("Player2Lines").GetComponent<Text>();
        scorePerLineText = GameObject.Find("ScorePerLine").GetComponent<Text>();
        scoreTotalText = GameObject.Find("ScoreTotal").GetComponent<Text>();
        Button rematchButton = GameObject.Find("RematchButton").GetComponent<Button>();
        rematchButton.onClick.AddListener(Rematch);
    }

    public void AddData(string p1Lines, string p2Lines, int[] scores)
    {
        p1LinesText.text = p1Lines;
        p2LinesText.text = p2Lines;

        for (int i = 0; i < scores.Length - 1; i++)
        {
            scorePerLineText.text = scorePerLineText.text + scores[i].ToString() + "\n";
        }
        scorePerLineText.text = scorePerLineText.text + scores[scores.Length - 1].ToString();
        scoreTotalText.text = scores.Aggregate((sum, next) => sum += next).ToString();
    }

    private void Rematch()
    {
        SceneManager.LoadScene("main");
    }
}
