using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    private void Awake () {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public void LaunchMultiPlayer () {
        MyNetworkManager.ExpectedPlayerCount = 2;
        SceneManager.LoadScene("main");
    }

    public void LaunchSinglePlayer () {
        MyNetworkManager.ExpectedPlayerCount = 1;
        SceneManager.LoadScene("main");
    }

    public void QuitGame () {
        Application.Quit();
    }
}
