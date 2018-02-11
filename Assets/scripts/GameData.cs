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
        private static GameData instance;
        [SerializeField]
        private List<Library> libraries;
        [SerializeField]
        private string selectedLibrary;
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

        public void SetSelectedLibrary(string name) {
            selectedLibrary = name;
        }

        public Library GetSelectedLibrary () {
            foreach (var lib in libraries) {
                if (lib.name.Equals(selectedLibrary)) {
                    return lib;
                }
            }
            return null;
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
        
        public void NewGame (string selectedLibrary, int roomSize, int gameLength, int seaSize, int lineLength) {
            this.selectedLibrary = selectedLibrary;
            this.roomSize = roomSize;
            this.gameLength = gameLength;
            this.seaSize = seaSize;
            this.lineLength = lineLength;
            Save();
        }

        public void AddLibrary (Library library) {
            libraries.Add(library);
            Save();
        }
    }
}
