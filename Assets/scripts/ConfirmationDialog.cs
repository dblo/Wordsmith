using System;
using UnityEngine;

namespace OO {
    public class ConfirmationDialog : MonoBehaviour {
        private Action yesAcition;

        public void SetOnConfirmAction (Action action) {
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