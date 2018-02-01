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
    }
}
