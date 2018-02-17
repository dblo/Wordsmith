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

        public string[] GetChoices (int index) {
            return content[index].Choices;
        }

        public GameData.LibraryType GetLibraryType() {
            if (content.Count == 1)
                return GameData.LibraryType.Free;
            else if (content[0] is SeaContent)
                return GameData.LibraryType.Fixed;
            //else if (content[0] is Query)
            //    return GameData.LibraryType.Query;
            else
                throw new InvalidOperationException("Unhandled library type");
        }

        public string GetAllChoices () {
            string res = "";
            for (int i = 0;;) {
                res += string.Join(" ", content[i].Choices);

                i++;
                if (i < content.Count) {
                    res += "\n\n";
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
