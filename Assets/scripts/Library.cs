using System;
using UnityEngine;

namespace OO {
    [Serializable]
    public class Library {
        [SerializeField] public string name;
        [SerializeField] public bool playerMade;
        [SerializeField] public string[] words;

        private static readonly Color PLAYER_MADE_COLOR = Color.white;
        private static readonly Color DEVELOPER_MADE_COLOR = Color.red;

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
