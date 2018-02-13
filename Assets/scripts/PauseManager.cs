using UnityEngine;

namespace OO {
    public class PauseManager : MonoBehaviour {
       [SerializeField] private GameObject pauseMenuPrefab;

        private GameObject pauseMenu;

        public void ToggleShowPauseMenu () {
            if (GamePaused()) {
                Destroy(pauseMenu);
            } else {
                pauseMenu = Instantiate(pauseMenuPrefab, transform.parent);
            }
        }

        private bool GamePaused () {
            return pauseMenu != null;
        }
    }
}
