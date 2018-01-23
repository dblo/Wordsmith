using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void ResumeGame()
    {
        Destroy(gameObject);
    }

    public void LaunchMainMenu()
    {
        SceneManager.LoadScene("main_menu");
    }
}
