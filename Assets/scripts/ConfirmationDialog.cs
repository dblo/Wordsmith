using System;
using UnityEngine;
using UnityEngine.UI;

namespace OO {
    public class ConfirmationDialog : MonoBehaviour {
        private Action yesAcition;

        public static void Create (string text, Action action) {
            var canvas = FindObjectOfType<Canvas>();
            var go = (GameObject) Instantiate(Resources.Load("ConfirmationDialog"), canvas.transform);
            var textComp = go.GetComponentInChildren<Text>();
            textComp.text = text;
            go.GetComponent<ConfirmationDialog>().SetOnConfirmAction(action);
        }

        public void SetOnConfirmAction (Action action) { //todo private
            yesAcition = action;
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