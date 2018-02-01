using UnityEngine;
using UnityEngine.Networking;

public class PauseMenu : MonoBehaviour {
    public void ResumeGame () {
        PauseManager pm = GameObject.Find("PauseButton").GetComponent<PauseManager>();
        pm.ToggleShowPauseMenu();
    }

    public void LaunchMainMenu () {
        var gm = GameObject.Find("GameManager");
        gm.GetComponent<GameManager>().LaunchMainMenu();
    }
}
