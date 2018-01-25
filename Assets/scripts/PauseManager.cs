using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuPrefab;

    private GameObject pauseMenu;
    private GameManager gm;

    private void Awake()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void ToggleShowPauseMenu()
    {
        if (GamePaused())
        {
            Destroy(pauseMenu);
            gm.SetUIButtonsInteractable(true);
        }
        else
        {
            gm.SetUIButtonsInteractable(false);
            GameObject canvas = GameObject.Find("Canvas");
            pauseMenu = Instantiate(pauseMenuPrefab, canvas.transform);
        }
    }

    private bool GamePaused()
    {
        return pauseMenu != null;
    }
}

