using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    public void ResumeGame () {
        PauseManager pm = GameObject.Find("PauseButton").GetComponent<PauseManager>();
        pm.ToggleShowPauseMenu();
    }

    public void LaunchMainMenu () {
        var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        nm.StopClient();
    }
}
