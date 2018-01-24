using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void ResumeGame()
    {
        PauseManager pm = GameObject.Find("PauseButton").GetComponent<PauseManager>();
        pm.ToggleShowPauseMenu();
    }

    public void LaunchMainMenu()
    {
        SceneManager.LoadScene("main_menu");
    }
}
