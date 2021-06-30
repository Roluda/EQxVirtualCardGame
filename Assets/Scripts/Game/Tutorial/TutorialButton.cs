using EQx.Game.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EQx.Game.Tutorial {
    public class TutorialButton : Button {
        [SerializeField]
        BlinkingImage blinkingImage = default;

        public void StartBlink() {
            blinkingImage.StartBlink();
        }

        public void StopBlink() {
            blinkingImage.StopBlink();
        }

        protected override void Awake() {
            base.Awake();
            onClick.AddListener(StopBlink);
        }

    }
}
