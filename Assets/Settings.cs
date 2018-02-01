using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {
    private void Awake () {
        var musicMuted = PlayerPrefs.GetInt("MusicMuted") > 0;
        var musicToggle = GameObject.Find("MusicToggle").GetComponent<Toggle>();
        musicToggle.isOn = musicMuted;
        musicToggle.onValueChanged.AddListener(delegate { SetMusicMuted(musicToggle); });

        var soundMuted = PlayerPrefs.GetInt("SoundMuted") > 0;
        var soundToggle = GameObject.Find("SoundToggle").GetComponent<Toggle>();
        soundToggle.isOn = soundMuted;
        soundToggle.onValueChanged.AddListener(delegate { SetSoundMuted(soundToggle); });
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
        int numVal = toggle.isOn ? 1 : 0;
        PlayerPrefs.SetInt("MusicMuted", numVal);
    }

    private void SetSoundMuted (Toggle toggle) {
        int numVal = toggle.isOn ? 1 : 0;
        PlayerPrefs.SetInt("SoundMuted", numVal);
        var gmGO = GameObject.Find("GameManager");
        if (gmGO != null) {
            var gm = gmGO.GetComponent<GameManager>();
            gm.SetSoundMuted(toggle.isOn);
        }
    }
}
