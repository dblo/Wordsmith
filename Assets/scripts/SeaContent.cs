using System;
using UnityEngine;

namespace OO {
    [Serializable]
    public class SeaContent {
        [SerializeField] private string[] choices;
        public string[] Choices { get { return choices; } }

        public SeaContent (string[] choices) {
            this.choices = choices;
        }
    }
}
