using UnityEngine;

public class PauseMenu : MonoBehaviour {
    public void ResumeGame () {
        PauseManager pm = GameObject.Find("PauseButton").GetComponent<PauseManager>();
        pm.ToggleShowPauseMenu();
    }

    public void LaunchMainMenu () {
        var parent = GameObject.Find("Canvas").transform;
        var go = (GameObject) Instantiate(Resources.Load("ConfirmationDialog"), parent);
        go.GetComponent<ConfirmationDialog>().SetOnConfirmAction(() => {
            var gm = GameObject.Find("GameManager");
            gm.GetComponent<GameManager>().LaunchMainMenu();
        });
    }

    public void LaunchSettings () {
        var parent = GameObject.Find("Canvas").transform;
        Instantiate(Resources.Load("SettingsDialog"), parent);
    }
}
