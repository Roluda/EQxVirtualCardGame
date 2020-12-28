using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EQx.Game.CountryCards {
    public class CountryName : CountryCardComponent {
        // Start is called before the first frame update
        [SerializeField]
        Image flagIcon = default;
        [SerializeField]
        TMP_Text label = default;
        [SerializeField]
        string path = "Flags";

        protected override void NewCardDataListener() {
            label.text = observedCard.data.countryName;
            flagIcon.sprite = Resources.Load<Sprite>(path + "/" + observedCard.data.isoCountryCode.ToLower());
        }
    }
}
