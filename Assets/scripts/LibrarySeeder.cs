using System;
using System.Collections.Generic;

namespace OO {
    public static class LibrarySeeder {

        public static void Seed () {
            string[] nouns = { "time","person","year","way","day","thing","man","world","life","hand","part","child","eye","woman","place","work","week","case","point","company","number" };
            string[] verbs = { "be","have","do","say","get","make","go","know","take","see","come","think","look","want","give","use","find","tell","ask","work","seem","feel","try","leave","call" };
            string[] adjectives = { "good", "first", "new", "last", "long", "great", "little", "own", "other", "old", "right", "big", "high", "different", "small", "large", "next", "early", "young", "important", "few", "public", "bad", "same", "able" };
            string[] prepositions = { "to", "of", "in", "for", "on", "with", "at", "by", "from", "up", "about", "into", "over", "after" };

            var choices = new string[nouns.Length + verbs.Length + adjectives.Length + prepositions.Length];
            Array.Copy(nouns, choices, nouns.Length);
            var ix = nouns.Length;
            Array.Copy(verbs, 0, choices, ix, verbs.Length);
            ix += verbs.Length;
            Array.Copy(adjectives, 0, choices, ix, adjectives.Length);
            ix += adjectives.Length;
            Array.Copy(prepositions, 0, choices, ix, prepositions.Length);

            var content = new SeaContent(choices);
            GameData.Instance.AddLibrary(new Library("Default", false, new List<SeaContent>() { content }));
        }
    }
}
