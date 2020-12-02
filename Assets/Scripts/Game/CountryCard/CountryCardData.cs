using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.CountryCards {
    public class CountryCardData {
        /// <summary>
        /// values requires an array of size 20
        /// </summary>
        /// <param name="countryName"></param>
        /// <param name="values"></param>
        public CountryCardData(string countryName, float[] values) {
            this.values = new Dictionary<EQxVariable, float>();
            for (int i=0; i<values.Length; i++) {
                this.values.Add((EQxVariable)i, values[i]);
                this.values[(EQxVariable)i] = values[i];
            }
            this.country = countryName;
        }

        public string country;

        public Dictionary<EQxVariable, float> values;

        public float GetValue(EQxVariable variable) {
            if (values.ContainsKey(variable)) {
                return values[variable];
            } else {
                return 0;
            }
        }
    }
}

