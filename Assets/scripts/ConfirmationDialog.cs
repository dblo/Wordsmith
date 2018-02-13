using System;
using UnityEngine;
using UnityEngine.UI;

namespace OO {
    public class ConfirmationDialog : MonoBehaviour {
        private Action yesCallback;

        public static void Create (string text, Action action, Transform parent) {
            var go = (GameObject) Instantiate(Resources.Load("ConfirmationDialog"), parent);
            var textComp = go.GetComponentInChildren<Text>();
            textComp.text = text;
            go.GetComponent<ConfirmationDialog>().yesCallback = action;
        }

        public void OnClickYes () {
            yesCallback();
            Destroy(gameObject);
        }

        public void OnClickNo () {
            Destroy(gameObject);
        }
    }
}
