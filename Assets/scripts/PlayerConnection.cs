using System;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnection : NetworkBehaviour {
    public string PlayerName { get; private set; }
    public string[] Words { get; private set; }

    private GameManager gm;

    public bool Ready {
        get { return Words != null; }
    }

    private void Start () {
        var go = GameObject.Find("GameManager");
        gm = go.GetComponent<GameManager>();

        if (isLocalPlayer || GameManager.ExpectedPlayerCount == 1) {
            go = GameObject.Find("ButtonBar");
            var bb = go.GetComponent<ButtonBar>();
            bb.AssignLocalPlayer(this);

            var playerName = PlayerPrefs.GetString(PreferencesKeys.PlayerName);
            if (playerName.Equals("")) {
                playerName = "Player " + new System.Random().Next(100);
                PlayerPrefs.SetString(PreferencesKeys.PlayerName, playerName);
            }
            CmdLocalPlayerReady(playerName);
        }
    }

    public void Reset () {
        Words = null;
    }

    [Command]
    private void CmdLocalPlayerReady (string playerName) {
        PlayerName = playerName;
        gm.PlayerSetupDone(this);
    }

    [Command]
    public void CmdSynchronizeName () {
        RpcSyncronizeName(PlayerName);
    }

    [ClientRpc]
    private void RpcSyncronizeName (string playerName) {
        PlayerName = playerName;
    }

    public void WordsChosen (string[] words) {
        gm.AddTemporaryWordsToLineLog(words);
        CmdWordsChosen(words);
    }

    [Command]
    private void CmdWordsChosen (string[] words) {
        Words = words;
        gm.CmdPlayerReady();
    }

    [Command]
    public void CmdSynchronizeWords () {
        RpcSynchronizeWords(Words);
    }

    [ClientRpc]
    private void RpcSynchronizeWords (string[] words) {
        Words = words;
    }
}
