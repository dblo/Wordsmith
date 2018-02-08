﻿using UnityEngine;
using UnityEngine.UI;

namespace OO {
    public class SettingsMenu : MonoBehaviour {
        private void Awake () {
            var musicMuted = Preferences.GetBool(Preferences.MusicMuted);
            var musicToggle = GameObject.Find("MusicToggle").GetComponent<Toggle>();
            musicToggle.isOn = musicMuted;
            musicToggle.onValueChanged.AddListener(delegate { SetMusicMuted(musicToggle); });

            var soundMuted = Preferences.GetBool(Preferences.SoundMuted);
            var soundToggle = GameObject.Find("SoundToggle").GetComponent<Toggle>();
            soundToggle.isOn = soundMuted;
            soundToggle.onValueChanged.AddListener(delegate { SetSoundMuted(soundToggle); });

            var clearPrefsBtn = GameObject.Find("ClearPrefsButton").GetComponent<Button>();
            clearPrefsBtn.onClick.AddListener(() => PlayerPrefs.DeleteAll());

            var playerNameInput = GameObject.Find("PlayerNameInput").GetComponent<InputField>();
            var playerName = PlayerPrefs.GetString(Preferences.PlayerName);
            if (playerName != "") {
                playerNameInput.text = playerName;
            }
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

        public void SetPlayerName (string name) {
            PlayerPrefs.SetString(Preferences.PlayerName, name);
        }

        private void SetMusicMuted (Toggle toggle) {
            var am = GameObject.Find("AudioManager").GetComponent<AudioSource>();
            if (toggle.isOn) {
                am.Stop();
            } else {
                am.Play();
            }
            Preferences.Set(Preferences.MusicMuted, toggle.isOn);
        }

        private void SetSoundMuted (Toggle toggle) {
            Preferences.Set(Preferences.SoundMuted, toggle.isOn);
            var gmGO = GameObject.Find("GameManager");
            if (gmGO == null)
                return; // Settings opened via main menu, no sound effects to handle atm
            gmGO.GetComponent<GameManager>().SetSoundMuted(toggle.isOn);
        }
    }
}
