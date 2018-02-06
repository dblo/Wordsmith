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
        }

        public void OnClose () {
            Destroy(gameObject);
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
            if (gmGO != null) {
                // Atm there are no sound effect outside game scene so this is sufficient
                var gm = gmGO.GetComponent<GameManager>();
                gm.SetSoundMuted(toggle.isOn);
            }
        }
    }
}
