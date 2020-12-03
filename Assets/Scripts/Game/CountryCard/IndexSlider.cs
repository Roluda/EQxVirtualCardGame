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
                dirty = false;
                float creation = observedCard.data.GetValue(EQxVariableType.ValueCreation);
                float extraction = observedCard.data.GetValue(EQxVariableType.ValueExtraction);
                float value = creation / (creation + extraction);
                if (float.IsNaN(value)) {
                    return;
                }
                slider.value = value;
            }
        }
    }
}
