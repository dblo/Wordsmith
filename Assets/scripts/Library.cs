using System;
using UnityEngine;

namespace OO {
    [Serializable]
    public class Library {
        [SerializeField] private string name;
        [SerializeField] private bool playerMade;
        [SerializeField] private string[] words;
        private static readonly Color PLAYER_MADE_COLOR = Color.white;
        private static readonly Color DEVELOPER_MADE_COLOR = Color.red;

        public string Name { get { return name; } }
        public bool PlayerMade { get { return playerMade; } }
        public string[] Words { get { return words; } }

        public Library (string name, bool playerMade, string[] words) {
            this.name = name;
            this.playerMade = playerMade;
            this.words = words;
        }

        public Color GetColor () {
            return playerMade ? PLAYER_MADE_COLOR : DEVELOPER_MADE_COLOR;
        }
    }
}
