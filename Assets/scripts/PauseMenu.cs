using UnityEngine;

namespace OO {
    public class PauseMenu : MonoBehaviour {
        public void ResumeGame () {
            PauseManager pm = GameObject.Find("PauseButton").GetComponent<PauseManager>();
            pm.ToggleShowPauseMenu();
        }

        public void LaunchMainMenu () {
            ConfirmationDialog.Create("Leave match?", () => GameManager.LaunchMainMenu(), transform.parent);
        }

        public void LaunchSettings () {
            Instantiate(Resources.Load("SettingsDialog"), transform.parent);
        }
    }
}
