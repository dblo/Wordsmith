using System;
using System.Collections.Generic;
using UnityEngine;

namespace OO {
    [Serializable]
    public class Library {
        [SerializeField] private string name;
        [SerializeField] private bool playerMade;
        [SerializeField] private List<SeaContent> content;
        private static readonly Color PLAYER_MADE_COLOR = Color.white;
        private static readonly Color DEVELOPER_MADE_COLOR = Color.red;

        public string Name { get { return name; } }
        public bool PlayerMade { get { return playerMade; } }
        public int RoundCount { get { return content.Count; } }

        public LibraryType Type {
            get {
                if (content.Count == 1)
                    return LibraryType.Free;
                else if (content[0] is SeaContent)
                    return LibraryType.Fixed;
                else
                    throw new InvalidOperationException("Unhandled library type");
            }
        }

        public string[] GetSea (int index) {
            return content[index].Choices;
        }

        public string GetAllChoices () {
            string res = "";
            for (int i = 0; ;) {
                res += string.Join(" ", content[i].Choices);

                i++;
                if (i < content.Count) {
                    res += "\n";
                } else
                    break;
            }
            res.TrimEnd();
            return res;
        }

        public Color Color {
            get { return playerMade ? PLAYER_MADE_COLOR : DEVELOPER_MADE_COLOR; }
        }

        public Library (string name, bool playerMade, List<SeaContent> content) {
            this.name = name;
            this.playerMade = playerMade;
            this.content = content;
        }
    }
}
