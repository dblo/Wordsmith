using System;

namespace OO {
    [Serializable]
    public static class GameData {
        public static Library Library { get; private set; }
        public static int PlayerCount { get; private set; }
        public static int GameLength { get; private set; }
        public static int SeaSize { get; private set; }
        public static int Picks { get; private set; }

        public static void SetSeaSize (int size) {
            SeaSize = size;
        }

        public static void SetPicks (int picks) {
            Picks = picks;
        }

        public static void NewGame (Library library, int playerCount, int gameLength,
            int seaSize, int picks) {
            Library = library;
            PlayerCount = playerCount;
            GameLength = gameLength;
            SeaSize = seaSize;
            Picks = picks;
        }
    }
}
