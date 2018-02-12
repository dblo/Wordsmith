using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

namespace OO {
    [Serializable]
    public class GameData {
        public static GameData Instance {
            get {
                if (instance == null) {
                    instance = Load(Application.persistentDataPath + "/gamedata.dat");
                }
                return instance;
            }
        }
        public Library SelectedLibrary { get; private set; }

        private static GameData instance;
        [SerializeField]
        private List<Library> libraries; //switch to set?
        [SerializeField]
        private int roomSize;
        [SerializeField]
        private int gameLength;
        [SerializeField]
        private int seaSize;
        [SerializeField]
        private int lineLength;
        private string savePath;

        public List<Library> GetLibraries () {
            return libraries;
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

        public static GameData Load (string path) {
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
            foreach (var lib in libraries) {
                if (lib == library) {
                    libraries.Remove(lib);
                    Save();
                    return;
                }
            }
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
            return libraries.Find(l => l.name.Equals(name));
        }

        public void ReplaceLibrary (Library library) {
            for (int i = 0; i < libraries.Count; i++) {
                if (libraries[i].name.Equals(library.name)) {
                    libraries[i] = library;
                    Save();
                    return;
                }
            }
            throw new InvalidOperationException("No library to replace was found.");
        }

        public void AddLibrary (Library library) {
            libraries.Add(library);
            Save();
        }
    }
}
