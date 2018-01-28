using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Networking;

public class GameOverScreen : MonoBehaviour {
    private void Awake () {
        Button rematchButton = GameObject.Find("RematchButton").GetComponent<Button>();
        rematchButton.onClick.AddListener(Rematch);

        Button mainMenuButton = GameObject.Find("MainMenuButton").GetComponent<Button>();
        mainMenuButton.onClick.AddListener(LaunchMainMenu);
    }

    public void AddData (string p1Name, string p2Name, string p1Lines, string p2Lines, int[] scores) {
        Text p1LinesText = GameObject.Find("Player1Lines").GetComponent<Text>();
        Text p2LinesText = GameObject.Find("Player2Lines").GetComponent<Text>();
        Text scorePerLineText = GameObject.Find("ScorePerLine").GetComponent<Text>();
        Text scoreTotalText = GameObject.Find("ScoreTotal").GetComponent<Text>();
        Text player1NameText = GameObject.Find("Player1Name").GetComponent<Text>();
        Text player2NameText = GameObject.Find("Player2Name").GetComponent<Text>();

        player1NameText.text = p1Name;
        player2NameText.text = p2Name;

        p1LinesText.text = p1Lines;
        p2LinesText.text = p2Lines;

        for (int i = 0; i < scores.Length - 1; i++) {
            scorePerLineText.text = scorePerLineText.text + scores[i].ToString() + "\n";
        }
        scorePerLineText.text = scorePerLineText.text + scores[scores.Length - 1].ToString();

        scoreTotalText.text = scores.Aggregate((sum, next) => sum += next).ToString();
    }

    private void Rematch () {
        //SceneManager.LoadScene("main");
    }

    private void LaunchMainMenu () {
        var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        var gm = GameObject.Find("GameManager");

        var nb = gm.GetComponent<NetworkBehaviour>();
        if (nb.isServer)
            nm.StopHost();
        else
            nm.StopClient();
    }
}
