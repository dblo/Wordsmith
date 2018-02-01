using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager instance;

    void Awake () {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        var musicMuted = PlayerPrefs.GetInt(PreferencesKeys.MusicMuted, 0) > 0;
        if(!musicMuted) {
            var audioSource = GetComponent<AudioSource>();
            audioSource.Play();
        }
    }
}
