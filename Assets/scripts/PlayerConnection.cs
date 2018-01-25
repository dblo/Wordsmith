using System;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnection : NetworkBehaviour
{
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
        }
        if(isServer)
        {
            gm.CmdAddPlayerName(netId.ToString());
        }
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
            playerName = netId.Value.ToString()
        };
        var jsonPkg = JsonUtility.ToJson(pkg);
        gm.CmdLineChosen(jsonPkg);
    }
}
