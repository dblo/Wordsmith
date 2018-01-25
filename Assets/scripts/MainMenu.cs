using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LaunchMultiPlayer()
    {
        MyNetworkManager.PlayerCount = 2;
        SceneManager.LoadScene("main");
    }

    public void LaunchSinglePlayer()
    {
        MyNetworkManager.PlayerCount = 1;
        SceneManager.LoadScene("main");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
