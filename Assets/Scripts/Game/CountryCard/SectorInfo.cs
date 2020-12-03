using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace EQx.Game.CountryCards {
    public class SectorInfo : CountryCardComponent {
        // Start is called before the first frame update

        [SerializeField]
        EQxVariableData sectorVariable = default;

        [SerializeField]
        TMP_Text head;

        [SerializeField]
        TMP_Text value;

        protected override void Validate() {
            head.text = sectorVariable.variableName;
            head.color = sectorVariable.color;
            value.color = sectorVariable.color;
            name = sectorVariable.variableName + "Info";
        }

        protected override void NewCardDataListener() {
            value.text = ((int)observedCard.data.GetValue(sectorVariable.variable)).ToString();
        }
    }
}
