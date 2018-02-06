using UnityEngine;

namespace OO {
    public class AudioManager : MonoBehaviour {
        public static AudioManager instance;

        void Awake () {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            var musicMuted = Preferences.GetBool(Preferences.MusicMuted);
            if (!musicMuted) {
                var audioSource = GetComponent<AudioSource>();
                audioSource.Play();
            }
        }
    }

}