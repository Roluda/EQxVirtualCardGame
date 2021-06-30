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
        [SerializeField]
        float secondScale = 0.9f;

        Color defaultColor = Color.white;
        [SerializeField]
        bool active = false;
        [SerializeField]
        RectTransform imageRect;



        private void OnValidate() {
            imageRect = image.rectTransform;
        }

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
                float value = (Mathf.Sin(Time.time * Mathf.PI * 2 * frequency) + 1) / 2;
                image.color = Color.Lerp(firstColor, secondColor, value);
                imageRect.localScale = Vector3.Lerp(Vector3.one, Vector3.one * secondScale, value);
            }
        }
    }
}
