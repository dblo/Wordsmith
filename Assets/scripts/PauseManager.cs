using UnityEngine;

public class PauseManager : MonoBehaviour {
    public GameObject pauseMenuPrefab;

    private GameObject pauseMenu;

    public void ToggleShowPauseMenu () {
        var gm = GameObject.Find("GameManager");
        if (GamePaused()) {
            Destroy(pauseMenu);
            if (gm != null)
                gm.GetComponent<GameManager>().SetUIButtonsInteractable(true);
        } else {
            if (gm != null)
                gm.GetComponent<GameManager>().SetUIButtonsInteractable(false);
            GameObject canvas = GameObject.Find("Canvas");
            pauseMenu = Instantiate(pauseMenuPrefab, canvas.transform);
        }
    }

    private bool GamePaused () {
        return pauseMenu != null;
    }
}
