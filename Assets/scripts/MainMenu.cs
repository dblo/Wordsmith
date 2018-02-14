using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace OO {
    public class MainMenu : MonoBehaviour {
        public static bool InLanMode { get; private set; }

        private void Awake () {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            var lanEnabled = Preferences.GetBool(Preferences.LAN_ENABLED);
            Toggle lanToggle = transform.Find("LanToggle").GetComponent<Toggle>();
            lanToggle.isOn = lanEnabled;
            SetLanMode(lanEnabled);
            lanToggle.onValueChanged.AddListener(SetLanMode);
        }

        public void LaunchLobby () {
            SceneManager.LoadScene("lobby");
        }

        public void LaunchSeaMaker () {
            SceneManager.LoadScene("seamaker");
        }

        public void JoinAnyGame () {
            var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

            if (InLanMode) {
                nm.StartClient();
            } else {
                NetworkManager.singleton.matchMaker.ListMatches(0, 1, "", true, 0, 0, OnMatchList);
            }
        }

        private static void OnMatchList (bool success, string extendedInfo, List<MatchInfoSnapshot> responseData) {
            if (!success || responseData.Count == 0)
                return;

            var match = responseData[0];
            var nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
            NetworkManager.singleton.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, nm.OnMatchJoined);
        }

        public void LaunchSettings () {
            var parent = GameObject.Find("Canvas").transform;
            Instantiate(Resources.Load("SettingsDialog"), parent);
        }

        private static void SetLanMode (bool value) {
            Preferences.SetBool(Preferences.LAN_ENABLED, value);
            InLanMode = value;

            if (InLanMode) {
                NetworkManager.singleton.StopMatchMaker();
            } else {
                NetworkManager.singleton.StartMatchMaker();
            }
        }
    }
}
