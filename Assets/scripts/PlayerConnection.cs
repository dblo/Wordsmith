using System;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnection : NetworkBehaviour
{
    // Currently this name is only set for the server instances
    public string playerName;

    private GameManager gm;

    private void Start()
    {
        var go = GameObject.Find("GameManager");
        gm = go.GetComponent<GameManager>();
        go = GameObject.Find("ButtonBar");

        if (isLocalPlayer)
        {
            var bb = go.GetComponent<ButtonBar>();
            bb.AssignLocalPlayer(this);

            var playerName = PlayerPrefs.GetString("name");
            if (playerName.Equals(""))
            {
                playerName = "Player " + new System.Random().Next(100);
                PlayerPrefs.SetString("name", playerName);
            }
            CmdPlayerReady(playerName);
        }
    }

    [Command]
    private void CmdPlayerReady(string playerName)
    {
        this.playerName = playerName;
        gm.CmdAddPlayer(netId.ToString());
    }

    public void WordsChosen(string[] words)
    {
        CmdOnWordsChosen(words);
    }

    [Command]
    private void CmdOnWordsChosen(string[] words)
    {
        var pkg = new PlayerStrings()
        {
            strings = words,
            playerID = netId.Value.ToString()
        };
        var jsonPkg = JsonUtility.ToJson(pkg);
        gm.CmdLineChosen(jsonPkg);
    }
}
