using UnityEngine;
using UnityEngine.Networking;

public class PauseMenu : MonoBehaviour {
    public void ResumeGame () {
        PauseManager pm = GameObject.Find("PauseButton").GetComponent<PauseManager>();
        pm.ToggleShowPauseMenu();
    }

    public void LaunchMainMenu () {
        var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        var gm = GameObject.Find("GameManager");
        var nb = gm.GetComponent<NetworkBehaviour>();
        if (nb.isServer)
            nm.StopHost();
        else
            nm.StopClient();
    }
}
