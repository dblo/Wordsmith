using UnityEngine;

namespace OO {
    public class AudioManager : MonoBehaviour {
        public static bool SoundMuted { get; set; }

        private void Awake () {
            DontDestroyOnLoad(gameObject);

            var musicMuted = Preferences.GetBool(Preferences.MUSIC_MUTED);
            if (!musicMuted) {
                var audioSource = GetComponent<AudioSource>();
                audioSource.Play();
            }
            SoundMuted = Preferences.GetBool(Preferences.SOUND_MUTED);
        }
    }
}
