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
        private List<Library> libraries;
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

        public void Save () {
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

        public void DeleteLibrary (string libraryName) {
            foreach (var lib in libraries) {
                if (lib.name.Equals(libraryName)) {
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

        public bool AddLibrary (Library library) {
            var existingLibrary = libraries.Find(l => l.name.Equals(library.name));
            if (existingLibrary == null) {
                libraries.Add(library);
            } else {
                if (existingLibrary.playerMade) {
                    existingLibrary = library; // todo warn overwrite
                } else {
                    return false; // todo tell user hw may not use default library names. Or allow overwrite as above.
                }
            }
            Save();
            return true;
        }
    }
}
