using System;
using System.Collections.Generic;
using System.Linq;

namespace OO {
    public class ColorMapper {
        public int Score { get; private set; }

        public const int GREEN_SCORE = 2;
        public const int YELLOW_SCORE = 1;
        private readonly List<string[]> playersWords;
        private readonly List<string[]> colors;

        public ColorMapper (int playerCount) {
            playersWords = new List<string[]>(GameManager.MAX_PLAYERS);
            colors = new List<string[]>(GameManager.MAX_PLAYERS);
        }

        // Returns list where element i contains the colors that correspond to the 
        // words for player i as sent to ComputeColors
        public List<string[]> GetColors () {
            return colors;
        }

        public void ComputeColors (List<PlayerConnection> players, int picks) {
            playersWords.Clear();
            colors.Clear();

            foreach (var p in players) {
                playersWords.Add((string[]) p.Words.Clone());
                colors.Add(GetInitialColors(picks));
            }
            for (var i = 0; i < picks; i++) {
                var word = playersWords[0][i];
                var j = 1;
                for (; j < players.Count; j++) {
                    if (!word.Equals(playersWords[j][i])) {
                        break;
                    }
                }
                if (j < players.Count)
                    continue;
                for (var k = 0; k < players.Count; k++) {
                    colors[k][i] = "green";
                    playersWords[k][i] = null;
                }
            }
            // Count the number of occurances for each word in all player's chosen words
            var wordDict = new Dictionary<string, uint>();
            foreach (var words in playersWords) {
                foreach (var word in words) {
                    if (word == null) {
                        //Do nothing
                    } else if (wordDict.ContainsKey(word)) {
                        wordDict[word] += 1;
                    } else {
                        wordDict[word] = 1;
                    }
                }
            }
            // Remove all words not chosen by all players
            foreach (var key in wordDict.Keys.ToList()) {
                if (wordDict[key] < players.Count)
                    wordDict.Remove(key);
            }
            foreach (var key in wordDict.Keys.ToList()) {
                MarkWordYellow(wordDict, key);
            }
        }

        private void MarkWordYellow (Dictionary<string, uint> wordDict, string key) {
            for (var i = 0; i < playersWords.Count; i++) {
                for (var j = 0; j < playersWords[i].Length; j++) {
                    if (!key.Equals(playersWords[i][j])) continue;

                    colors[i][j] = "yellow";
                    wordDict[key] -= 1;
                    if (wordDict[key] == 0) {
                        return;
                    }
                }
            }
        }

        public void ComputeScore () {
            foreach (var c in colors[0]) {
                if (c == "green") {
                    Score += GREEN_SCORE;
                } else if (c == "yellow") {
                    Score += YELLOW_SCORE;
                } // else red so grant no points
            }
        }

        private static string[] GetInitialColors (int length) {
            return Enumerable.Repeat("red", length).ToArray();
        }

        public static string[] GetTemporaryWordColors (int length) {
            return Enumerable.Repeat("white", length).ToArray();
        }
    }
}
