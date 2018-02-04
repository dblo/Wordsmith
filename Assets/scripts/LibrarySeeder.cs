using System;
using UnityEngine;

namespace OO {
    public class LibrarySeeder : MonoBehaviour {
        private void Start () {
            if (PrefsAlreadySeeded())
                return;
            SeedPreferences();
        }

        private static bool PrefsAlreadySeeded () {
            var libraryNames = PlayerPrefs.GetString(PreferencesKeys.DefaultLibraryNames);
            return libraryNames != "";
        }

        private void SeedPreferences () {
            string[] nouns = { "time","person","year","way","day","thing","man","world","life","hand","part","child","eye","woman","place","work","week","case","point","company","number" };
            string[] verbs = { "be","have","do","say","get","make","go","know","take","see","come","think","look","want","give","use","find","tell","ask","work","seem","feel","try","leave","call" };
            string[] adjectives = { "good", "first", "new", "last", "long", "great", "little", "own", "other", "old", "right", "big", "high", "different", "small", "large", "next", "early", "young", "important", "few", "public", "bad", "same", "able" };
            string[] prepositions = { "to", "of", "in", "for", "on", "with", "at", "by", "from", "up", "about", "into", "over", "after" };

            var library = new string[nouns.Length + verbs.Length + adjectives.Length + prepositions.Length];
            Array.Copy(nouns, library, nouns.Length);
            int ix = nouns.Length;
            Array.Copy(verbs, 0, library, ix, verbs.Length);
            ix += verbs.Length;
            Array.Copy(adjectives, 0, library, ix, adjectives.Length);
            ix += adjectives.Length;
            Array.Copy(prepositions, 0, library, ix, prepositions.Length);

            //Todo dont allow ; in library names
            var libraryNames = PlayerPrefs.GetString(PreferencesKeys.DefaultLibraryNames);
            if (libraryNames == "") {
                PlayerPrefs.SetString(PreferencesKeys.DefaultLibraryNames, "Default");
            } else {
                PlayerPrefs.SetString(PreferencesKeys.DefaultLibraryNames, libraryNames + 
                    GC.LibraryNameDelimiter + "Default");
            }

            var foo = JsonArrayHelper.ToJson(library);
            PlayerPrefs.SetString("Default", foo);
        }
    }
}
