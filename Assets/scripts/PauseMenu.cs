using UnityEngine;

namespace OO {
    public class PauseMenu : MonoBehaviour {
        public void ResumeGame () {
            PauseManager pm = GameObject.Find("PauseButton").GetComponent<PauseManager>();
            pm.ToggleShowPauseMenu();
        }

        public void LaunchMainMenu () {
            var go = (GameObject) Instantiate(Resources.Load("ConfirmationDialog"), transform.parent);
            go.GetComponent<ConfirmationDialog>().SetOnConfirmAction(() => {
                GameManager.LaunchMainMenu();
            });
        }

        public void LaunchSettings () {
            Instantiate(Resources.Load("SettingsDialog"), transform.parent);
        }
    }
}
