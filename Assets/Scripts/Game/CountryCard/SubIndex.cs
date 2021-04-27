using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace EQx.Game.CountryCards {
    public class SubIndex : CountryCardComponent {
        [SerializeField]
        EQxVariableData data = default;
        [SerializeField]
        TMP_Text value = default;
        [SerializeField]
        Image icon = default;

        [SerializeField]
        TMP_Text rank = default;
        [SerializeField]
        string rankPrefix = "Rank: ";


        protected override void Validate() {
            icon.sprite = data.iconTransparent;
        }
        // Start is called before the first frame update

        protected override void NewCardDataListener() {
            value.text = ((int)observedCard.data.GetValue(data.type)).ToString();
            rank.text = rankPrefix + CountryCardDatabase.instance.GetRank(observedCard.data, data.type);
        }
    }
}
