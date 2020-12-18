using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.CountryCards {
    public class ScaleOnHover : CountryCardComponent {

        [SerializeField, Range(0, 10)]
        float scaleFactor = 2;
        [SerializeField]
        int layer = 10;

        bool scaled = false;
        int oldLayer;

        private void OnMouseEnter() {
            if (!scaled) {
                scaled = true;
                oldLayer = observedCard.layer;
                observedCard.layer = layer;
                observedCard.transform.localScale *= scaleFactor;
            }

        }

        private void OnMouseExit() {
            if (scaled) {
                scaled = false;
                observedCard.layer = oldLayer;
                observedCard.transform.localScale /= scaleFactor;
            }

        }

        private void OnMouseDown() {
            if (!scaled) {
                scaled = true;
                oldLayer = observedCard.layer;
                observedCard.layer = layer;
                observedCard.transform.localScale *= scaleFactor;
            }
        }

        private void OnMouseUp() {
            if (!scaled) {
                scaled = false;
                observedCard.layer = oldLayer;
                observedCard.transform.localScale /= scaleFactor;
            }
        }
    }
}
