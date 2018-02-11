using System;
using UnityEngine;

namespace OO {
    [Serializable]
    public class Library {
        public string name;
        public bool playerMade;
        public string[] words;

        private static readonly Color playerMadeColor = Color.white;
        private static readonly Color developerMadeColor = Color.red;

        public Color GetColor() {
            return playerMade ? playerMadeColor : developerMadeColor;
        }

        public static Color GetColor (bool playerMade) {
            return playerMade ? playerMadeColor : developerMadeColor;
        }
    }
}
