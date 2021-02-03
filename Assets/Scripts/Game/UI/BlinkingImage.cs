using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EQx.Game.UI {
    public class BlinkingImage : MonoBehaviour {

        [SerializeField]
        Image image = default;
        [SerializeField]
        float frequency = 1;
        [SerializeField]
        Color firstColor = Color.white;

        [SerializeField]
        Color secondColor = Color.yellow;

        Color defaultColor = Color.white;
        bool active = false;

        public void StartBlink() {
            if (!active) {
                defaultColor = image.color;
                active = true;
            }
        }

        public void StopBlink() {
            if (active) {
                active = false;
                image.color = defaultColor;
            }
        }

        // Update is called once per frame
        void Update() {
            if (active) {
                image.color = Color.Lerp(firstColor, secondColor, (Mathf.Sin(Time.time * Mathf.PI * 2 * frequency) + 1) / 2);
            }
        }
    }
}
