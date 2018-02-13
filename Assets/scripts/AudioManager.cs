using UnityEngine;

namespace OO {
    public class AudioManager : MonoBehaviour {
        private void Awake () {
            DontDestroyOnLoad(gameObject);

            var musicMuted = Preferences.GetBool(Preferences.MUSIC_MUTED);
            if (!musicMuted) {
                var audioSource = GetComponent<AudioSource>();
                audioSource.Play();
            }
        }
    }

}