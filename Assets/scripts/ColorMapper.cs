using System;
using System.Collections.Generic;
using System.Linq;

namespace OO {
    public class ColorMapper {
        private const int PERFECT_SCORE_BONUS = 2;
        private readonly List<string[]> playersWords;
        private readonly List<string[]> colors;
        private readonly List<int> playersScore;

        public ColorMapper (int playerCount) {
            playersScore = new List<int>(Enumerable.Repeat(0, playerCount).ToArray());
            playersWords = new List<string[]>(playerCount);
            colors = new List<string[]>(playerCount);
        }

        public List<int> Scores {
            get { return playersScore; }
        }

        // Returns list where element i contains the colors that correspond to the 
        // words for player i as sent to ComputeColors
        public List<string[]> GetColors () {
            return colors;
        }

        public void ComputeColors (List<PlayerConnection> players) {
            playersWords.Clear();

            foreach (var p in players) {
                playersWords.Add((string[]) p.Words.Clone());
                colors.Add(GetInitialColors(GameData.Instance.GetLineLength()));
            }
            for (var i = 0; i < GameData.Instance.GetLineLength(); i++) {
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
            // Remove all words not chosen by at least 2 players
            foreach (var key in wordDict.Keys.ToList()) {
                if (wordDict[key] < 2)
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
            for (var i = 0; i < colors.Count; i++) {
                var score = 0;
                foreach (var c in colors[i]) {
                    if (c == "green") {
                        score += 2;
                    } else if (c == "yellow") {
                        score += 1;
                    } // else red so grant no points
                }
                if (PerfectScore(GameData.Instance.GetLineLength(), score))
                    score += PERFECT_SCORE_BONUS;
                playersScore[i] += score;
            }
        }

        private static string[] GetInitialColors (int length) {
            return Enumerable.Repeat("red", length).ToArray();
        }

        public static string[] GetTemporaryWordColors (int length) {
            return Enumerable.Repeat("white", length).ToArray();
        }

        // True if score is such that 2 points per word was awarded
        private static bool PerfectScore (int wordCount, int score) {
            return score == wordCount * 2;
        }
    }
}
