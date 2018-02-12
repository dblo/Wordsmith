using System;
using UnityEngine;
using UnityEngine.UI;

namespace OO {
    public class ConfirmationDialog : MonoBehaviour {
        private Action yesAcition;

        public static void Create (string text, Action action, Transform parent) {
            var go = (GameObject) Instantiate(Resources.Load("ConfirmationDialog"), parent);
            var textComp = go.GetComponentInChildren<Text>();
            textComp.text = text;
            go.GetComponent<ConfirmationDialog>().yesAcition = action;
        }

        public void OnClickYes () {
            yesAcition();
            Destroy(gameObject);
        }

        public void OnClickNo () {
            Destroy(gameObject);
        }
    }
}
