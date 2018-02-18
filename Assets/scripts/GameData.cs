using System;

namespace OO {
    [Serializable]
    public static class GameData {
        public static Library Library { get; private set; }
        public static int PlayerCount { get; private set; }
        public static int GameLength { get; private set; }
        public static int SeaSize { get; private set; }
        public static int Picks { get; private set; }
        public static int CurrentRound { get; private set; }
        public static int PrefferedPicks { get; private set; }

        public static void NewRound () {
            CurrentRound++;

            if (CurrentRound < GameLength)
                UpdatePicks();
        }

        public static void NewGame (Library library, int playerCount, int gameLength,
            int seaSize, int prefferedPicks) {
            Library = library;
            PlayerCount = playerCount;
            GameLength = gameLength;
            SeaSize = seaSize;
            CurrentRound = 0;
            PrefferedPicks = prefferedPicks;
            UpdatePicks();
        }

        private static void UpdatePicks () {
            Picks = Math.Min(Library.GetSea(CurrentRound).Length, PrefferedPicks);
        }
    }
}
