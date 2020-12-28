using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace EQx.Game.CountryCards {
    public class CountryCardDatabase : MonoBehaviour {

        public static CountryCardDatabase instance = null;
        [SerializeField]
        public EQxDataSet data = default;

        public int length { get => data.eqxCountryData.Length; }

        // Start is called before the first frame update
        void Awake() {
            if (instance) {
                Destroy(gameObject);
            } else {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }
        }

        public int GetIndex(EQxCountryData countryData) {
            return Array.IndexOf(data.eqxCountryData, countryData);
        }

        public EQxCountryData GetCountry(int id) {
            if(id >= data.eqxCountryData.Length) {
                return data.eqxCountryData[0];
            }
            return data.eqxCountryData[id];
        }

        public EQxCountryData GetCountry(string name) {
            return data.eqxCountryData.Where(country => country.countryName == name).FirstOrDefault();
        }
    }
}
