using System;
using UnityEngine;

namespace OO {
    [Serializable]
    public class Library {
        [SerializeField]
        private string name;
        [SerializeField]
        private bool playerMade;
        [SerializeField]
        private string[] words;

        private static readonly Color playerMadeColor = Color.white;
        private static readonly Color developerMadeColor = Color.red;

        public Library (string name, bool playerMade, string[] words) {
            this.name = name;
            this.playerMade = playerMade;
            this.words = words;
        }

        public string Name { get { return name; } }
        public bool PlayerMade { get { return playerMade; } }
        public string[] Words { get { return words; } }

        public Color GetColor () {
            return PlayerMade ? playerMadeColor : developerMadeColor;
        }

        public static Color GetColor (bool playerMade) {
            return playerMade ? playerMadeColor : developerMadeColor;
        }
    }
}
