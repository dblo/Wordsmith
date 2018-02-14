using UnityEngine;

namespace OO {
    public class AudioManager : MonoBehaviour {
        public static AudioManager Instance { get; private set; }
        public bool SoundMuted { get; private set; }
        private AudioSource audioSource;

        private void Awake () {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();

            SoundMuted = Preferences.GetBool(Preferences.SOUND_MUTED);
            var musicMuted = Preferences.GetBool(Preferences.MUSIC_MUTED);
            if (!musicMuted) {
                audioSource.Play();
            }
        }

        public void SetMusicMuted (bool value) {
            if (value) {
                audioSource.Stop();
            } else {
                audioSource.Play();
            }
            Preferences.SetBool(Preferences.MUSIC_MUTED, value);
        }

        public void SetSoundMuted (bool value) {
            SoundMuted = value;
            Preferences.SetBool(Preferences.SOUND_MUTED, value);
        }
    }
}
