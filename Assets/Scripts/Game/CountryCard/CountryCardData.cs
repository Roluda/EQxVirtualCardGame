using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace EQx.Game.CountryCards {
    [Serializable]
    public class EQxVariable {
        [SerializeField]
        public EQxVariableType type = default;
        [SerializeField, Range(0, 100)]
        public float value;
    }
    [CreateAssetMenu(fileName = "CountryData_New", menuName = "Data/Country_Data", order = 2)]
    public class CountryCardData : ScriptableObject {
        [SerializeField]
        public int cardID = 0;
        [SerializeField]
        public string country = "Default Island";
        [SerializeField]
        public Sprite flag = default;
        [SerializeField]
        public List<EQxVariable> variables = default;


        public float GetValue(EQxVariableType variable) {
            var eqxVar = variables.Where(var => var.type == variable).FirstOrDefault();
            if (eqxVar != null) {
                return eqxVar.value;
            } else {
                return 0;
            }
        }
    }
}

