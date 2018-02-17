using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OO {
    public enum LibraryType { Free, Fixed, Query }

    [Serializable]
    public class LibraryManager {
        private static LibraryManager instance;
        private string savePath;
        [SerializeField] private List<Library> libraries;

        public static LibraryManager Instance {
            get {
                return instance ?? (instance =
                           Load(Application.persistentDataPath + "/gamedata.dat"));
            }
        }
        public int LibraryCount { get { return libraries.Count; } }

        public Library GetLibrary (int ix) {
            return libraries[ix];
        }

        private void Save () {
            var json = JsonUtility.ToJson(this);
            File.WriteAllText(savePath, json);
        }

        private static LibraryManager Load (string path) {
            if (!File.Exists(path)) {
                return new LibraryManager() {
                    savePath = path,
                    libraries = new List<Library>()
                };
            }
            var json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<LibraryManager>(json);
            data.savePath = path;
            return data;
        }

        public void DeleteLibrary (Library library) {
            if (libraries.Remove(library)) {
                Save();
            } else
                throw new InvalidOperationException("Could not find library to be deleted.");
        }

        public Library FindLibrary (string name) {
            return libraries.Find(l => l.Name.Equals(name));
        }

        public void ReplaceLibrary (Library library) {
            for (var i = 0; i < libraries.Count; i++) {
                if (!libraries[i].Name.Equals(library.Name))
                    continue;
                libraries[i] = library;
                Save();
                return;
            }
            throw new InvalidOperationException("No library to replace was found.");
        }

        public void AddLibrary (Library library) {
            libraries.Add(library);
            Save();
        }
    }
}
