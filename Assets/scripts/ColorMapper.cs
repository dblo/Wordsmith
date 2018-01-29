using System.Collections.Generic;
using System.Linq;

public class ColorMapper {
    private const int perfectScoreBonus = 2;
    private readonly int wordsPerLine;
    private List<string[]> words;
    private List<string[]> colors;
    private List<int> scorePerLine = new List<int>();

    public ColorMapper (int wordsPerLine) {
        this.wordsPerLine = wordsPerLine;
    }

    public int[] GetScores () {
        return scorePerLine.ToArray();
    }

    // Returns list where element i contains the colors that correspond to the 
    // words for player i as sent to ComputeColors
    public List<string[]> GetColors () {
        return colors;
    }

    public void ComputeColors (List<PlayerConnection> players) {
        words = new List<string[]>(players.Count);
        colors = new List<string[]>(players.Count);

        foreach (var p in players) {
            words.Add((string[]) p.Words.Clone());
            colors.Add(GetInitialColors(wordsPerLine));
        }
        for (int i = 0; i < wordsPerLine; i++) {
            var word = words[0][i];
            int j = 1;
            for (; j < players.Count; j++) {
                if (!word.Equals(words[j][i])) {
                    break;
                }
            }
            if (j < players.Count)
                continue;
            for (int k = 0; k < players.Count; k++) {
                colors[k][i] = "green";
                words[k][i] = null;
            }
        }
        var wordDict = new Dictionary<string, uint>();
        foreach (var line in words) {
            foreach (var word in line) {
                if (word == null)
                    continue;
                else if (wordDict.ContainsKey(word))
                    wordDict[word] += 1;
                else
                    wordDict[word] = 1;
            }
        }
        foreach (var key in wordDict.Keys.ToList()) {
            if (wordDict[key] < players.Count)
                wordDict.Remove(key);
        }
        foreach (var key in wordDict.Keys.ToList()) {
            MarkWordYellow(wordDict, key);
        }
    }

    private void MarkWordYellow (Dictionary<string, uint> wordDict, string key) {
        for (int i = 0; i < wordsPerLine; i++) {
            for (int j = 0; j < words[i].Length; j++) {
                if (key.Equals(words[i][j])) {
                    colors[i][j] = "yellow";
                    wordDict[key] -= 1;
                    if (wordDict[key] == 0) {
                        return;
                    }
                }
            }
        }
    }

    public void ComputeScore (int playerIx) {
        int score = 0;
        foreach (var c in colors[playerIx]) {
            if (c == "green")
                score += 2;
            else if (c == "yellow")
                score += 1;
        }
        if (PerfectScore(wordsPerLine, score))
            score += perfectScoreBonus;
        scorePerLine.Add(score);
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
