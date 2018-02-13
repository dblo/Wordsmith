using UnityEngine;
using UnityEngine.UI;

namespace OO {
    public class LibraryListButton : MonoBehaviour {
        public Library Library { get; set; }

        public void Setup (Library library) {
            Library = library;

            var text = GetComponentInChildren<Text>();
            text.text = library.name;
            text.color = library.GetColor();
        }
    }
}
