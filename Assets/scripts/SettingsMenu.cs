using System;
using UnityEngine;
using UnityEngine.UI;

namespace OO {
    public class SettingsMenu : MonoBehaviour {
        private void Awake () {
            var musicMuted = Preferences.GetBool(Preferences.MUSIC_MUTED);
            var musicToggle = GameObject.Find("MusicToggle").GetComponent<Toggle>();
            musicToggle.isOn = musicMuted;
            musicToggle.onValueChanged.AddListener(AudioManager.Instance.SetMusicMuted);

            var soundMuted = Preferences.GetBool(Preferences.SOUND_MUTED);
            var soundToggle = GameObject.Find("SoundToggle").GetComponent<Toggle>();
            soundToggle.isOn = soundMuted;
            soundToggle.onValueChanged.AddListener(AudioManager.Instance.SetSoundMuted);

            var clearPrefsBtn = GameObject.Find("ClearPrefsButton").GetComponent<Button>();
            clearPrefsBtn.onClick.AddListener(PlayerPrefs.DeleteAll);

            var playerNameInput = GameObject.Find("PlayerNameInput").GetComponent<InputField>();
            var playerName = PlayerPrefs.GetString(Preferences.PLAYER_NAME);
            playerNameInput.text = playerName;

            if (GameObject.Find("GameManager") == null) {
                playerNameInput.onEndEdit.AddListener(SetPlayerName);
            } else {
                // We do not allow name changes during an on-going match
                playerNameInput.interactable = false;
            }
        }

        public void OnClose () {
            Destroy(gameObject);
        }

        private static void SetPlayerName (string name) {
            PlayerPrefs.SetString(Preferences.PLAYER_NAME, name);
        }

        public void SeedLibraries () {
            LibrarySeeder.Seed();
        }
    }
}
