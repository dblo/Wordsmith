using UnityEngine;
using UnityEngine.Networking;

namespace OO {
    public class PlayerConnection : NetworkBehaviour {
        public string PlayerName { get; private set; }
        public string[] Words { get; private set; }

        private GameManager gm;

        public bool Ready {
            get { return Words != null; }
        }

        private void Start () {
            // Need to init gm for all playerConnections on the host. Only for localPlayer necessary on clients.
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            if (!isLocalPlayer) return;

            var bb = GameObject.Find("ButtonBar").GetComponent<ButtonBar>();
            bb.AssignLocalPlayer(this);

            var playerName = PlayerPrefs.GetString(Preferences.PLAYER_NAME);
            if (playerName.Equals("")) {
                playerName = "Player " + new System.Random().Next(100);
                PlayerPrefs.SetString(Preferences.PLAYER_NAME, playerName);
            }
            CmdLocalPlayerReady(playerName);
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
}
