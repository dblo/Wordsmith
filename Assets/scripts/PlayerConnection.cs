using UnityEngine;
using UnityEngine.Networking;

namespace OO {
    public class PlayerConnection : NetworkBehaviour {
        public string PlayerName { get; private set; }
        public string[] Words { get; private set; }
        // All PlayerConnections on the server and the localPlayer instances 
        // on the clients will comunicate with their gm instance
        private GameManager gm;

        public bool Ready {
            get { return Words != null; }
        }

        private void Start () {
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            if (!isLocalPlayer)
                return;

            var bb = GameObject.Find("ButtonBar").GetComponent<ButtonBar>();
            bb.SetLocalPlayer(this);

            var playerName = PlayerPrefs.GetString(Preferences.PLAYER_NAME);
            if (playerName.Equals("")) {
                playerName = "Player " + new System.Random().Next(100);
                PlayerPrefs.SetString(Preferences.PLAYER_NAME, playerName);
            }
            CmdLocalPlayerReady(playerName);
        }

        public void NewRound () {
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
}
