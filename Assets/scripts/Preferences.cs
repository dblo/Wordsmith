using UnityEngine;

namespace OO {
    public class Preferences {
        public const string MUSIC_MUTED = "music_muted";
        public const string SOUND_MUTED = "sound_muted";
        public const string DEFAULT_PLAYER_COUNT = "default_player_count";
        public const string DEFAULT_GAME_LENGTH = "default_game_length";
        public const string DEFAULT_SEA_SIZE = "default_sea_size";
        public const string DEFAULT_LINE_LENGTH = "default_line_length";
        public const string PLAYER_NAME = "player_name";
        public const string LAN_ENABLED = "lan_enabled";

        public static void SetBool(string key, bool value) {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        public static bool GetBool(string key) {
            return PlayerPrefs.GetInt(key, 0) != 0;
        }
    }
}
