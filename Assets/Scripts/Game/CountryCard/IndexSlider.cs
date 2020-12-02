using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EQx.Game.CountryCards {
    public class IndexSlider : CountryCardComponent {
        [SerializeField]
        Slider slider = default;
        private bool dirty = false;

        protected override void NewCardDataListener() {
            dirty = true;
        }

        private void Update() {
            if (dirty) {
                Debug.Log("Slidign");
                dirty = false;
                float creation = observedCard.data.GetValue(EQxVariable.ValueCreation);
                float extraction = observedCard.data.GetValue(EQxVariable.ValueExtraction);
                float value = creation / (creation + extraction);
                if (float.IsNaN(value)) {
                    return;
                }
                slider.value = value;
            }
        }
    }
}
