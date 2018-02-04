using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

namespace OO {
    public class MainMenu : MonoBehaviour {
        public MultiplayerLobby lobbyPrefab;
        public SeaMaker seaMakerPrefab;
        public bool InLanMode { get { return lanToggle.isOn; } }

        private Toggle lanToggle;

        private void Awake () {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            var lanEnabled = PlayerPrefs.GetInt(PreferencesKeys.LanEnabled, 0);
            lanToggle = GameObject.Find("LanToggle").GetComponent<Toggle>();
            lanToggle.isOn = lanEnabled > 0;
            SetLanMode(lanEnabled > 0);
            lanToggle.onValueChanged.AddListener(SetLanMode);
        }

        public void LaunchLobby () {
            var canvas = GameObject.Find("Canvas");
            Instantiate(lobbyPrefab, canvas.transform);
        }

        public void LaunchSeaMaker () {
            var canvas = GameObject.Find("Canvas");
            Instantiate(seaMakerPrefab, canvas.transform);
        }

        public void JoinAnyGame () {
            GameManager.ExpectedPlayerCount = 2;
            var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

            if (InLanMode) {
                nm.StartClient();
            } else {
                NetworkManager.singleton.matchMaker.ListMatches(0, 1, "", true, 0, 0, OnMatchList);
            }
        }

        private void OnMatchList (bool success, string extendedInfo, List<MatchInfoSnapshot> responseData) {
            if (!success || responseData.Count == 0)
                return; // todo what?

            var match = responseData[0];
            var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
            NetworkManager.singleton.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, nm.OnMatchJoined);
        }

        public void LaunchSettings () {
            var parent = GameObject.Find("Canvas").transform;
            Instantiate(Resources.Load("SettingsDialog"), parent);
        }

        public void SetLanMode (bool value) {
            int lanEnabled = value ? 1 : 0;
            PlayerPrefs.SetInt(PreferencesKeys.LanEnabled, lanEnabled);

            if (InLanMode) {
                NetworkManager.singleton.StopMatchMaker();
            } else {
                NetworkManager.singleton.StartMatchMaker();
            }
        }
    }
}
