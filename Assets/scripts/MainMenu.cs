﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    private void Awake () {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public void LaunchMultiPlayer () {
        GameManager.ExpectedPlayerCount = 2;
        //SceneManager.LoadScene("main");
    }

    public void LaunchSinglePlayer () {
        GameManager.ExpectedPlayerCount = 1;
        //SceneManager.LoadScene("main");
    }

    public void QuitGame () {
        Application.Quit();
    }
}
