using System.Collections.Generic;
using UnityEngine;

namespace OO {
    public class Preferences {
        public const string MusicMuted = "music_muted";
        public const string SoundMuted = "sound_muted";
        public const string DefaultPlayerCount = "default_player_count";
        public const string DefaultGameLength = "default_game_length";
        public const string DefaultSeaSize = "default_sea_size";
        public const string DefaultLineLength = "default_line_length";
        public const string PlayerName = "player_name";
        public const string LanEnabled = "lan_enabled";

        public static string[] GetArray (string key) {
            var json = PlayerPrefs.GetString(key);
            if (json == "")
                return new string[0];

            var data = JsonArrayHelper.FromJson<string>(json);
            return data;
        }

        public static void SetArray(string key, string[] values) {
            var json = JsonArrayHelper.ToJson(values);
            PlayerPrefs.SetString(key, json);
        }

        public static void AddToArray (string key, string value) {
            var values = new List<string>(GetArray(key));
            if(value.Length == 0) {
                SetArray(key, new string[] { value });
            } else {
                values.Add(value);
                SetArray(key, values.ToArray());
            }
        }

        public static bool DeleteFromArray(string arrayKey, string elementKey) {
            var values = new List<string>(GetArray(arrayKey));
            return values.Remove(elementKey);
        }

        public static void Set(string key, bool value) {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        public static bool GetBool(string key) {
            return PlayerPrefs.GetInt(key) != 0;
        }
    }
}
