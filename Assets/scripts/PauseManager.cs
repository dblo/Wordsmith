using UnityEngine;

namespace OO {
    public class PauseManager : MonoBehaviour {
        public GameObject pauseMenuPrefab;

        private GameObject pauseMenu;

        public void ToggleShowPauseMenu () {
            if (GamePaused()) {
                Destroy(pauseMenu);
            } else {
                GameObject canvas = GameObject.Find("Canvas");
                pauseMenu = Instantiate(pauseMenuPrefab, canvas.transform);
            }
        }

        private bool GamePaused () {
            return pauseMenu != null;
        }
    }
}
