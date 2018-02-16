using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

namespace OO {
    [Serializable]
    public class GameData {
        public enum LibraryType { Free, Fixed, Query }

        public Library SelectedLibrary { get; private set; }
        public int LibraryCount { get { return libraries.Count; } }
        public static LibraryType ActiveLibraryType { get; private set; }

        private static GameData instance;
        [SerializeField] private List<Library> libraries;
        [SerializeField] private int roomSize;
        [SerializeField] private int gameLength;
        [SerializeField] private int seaSize;
        [SerializeField] private int lineLength;
        private string savePath;

        public static GameData Instance {
            get {
                return instance ?? (instance =
                           Load(Application.persistentDataPath + "/gamedata.dat"));
            }
        }

        public Library GetLibrary (int ix) {
            return libraries[ix];
        }

        public int GetRoomSize () {
            return roomSize;
        }

        public int GetGameLength () {
            return gameLength;
        }

        public int GetSeaSize () {
            return seaSize;
        }

        public int GetLineLength () {
            return lineLength;
        }

        private void Save () {
            var json = JsonUtility.ToJson(this);
            File.WriteAllText(savePath, json);
        }

        private static GameData Load (string path) {
            if (!File.Exists(path)) {
                return new GameData() {
                    savePath = path,
                    libraries = new List<Library>()
                };
            }
            var json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<GameData>(json);
            data.savePath = path;
            return data;
        }

        public void DeleteLibrary (Library library) {
            if (libraries.Remove(library)) {
                Save();
            } else
                throw new InvalidOperationException("Could not find library to be deleted.");
        }

        public void NewGame (Library selectedLibrary, int roomSize, int gameLength, int seaSize, int lineLength) {
            SelectedLibrary = selectedLibrary;
            this.roomSize = roomSize;
            this.gameLength = gameLength;
            this.seaSize = seaSize;
            this.lineLength = lineLength;
            Save();
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
