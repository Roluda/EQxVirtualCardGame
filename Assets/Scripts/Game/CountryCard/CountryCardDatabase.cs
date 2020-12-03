using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace EQx.Game.CountryCards {
    public class CountryCardDatabase : MonoBehaviour {

        [SerializeField]
        public List<CountryCardData> data;

        public CountryCardData GetCountry(int id) {
            return data.Where(country => country.cardID == id).FirstOrDefault();
        }

        public CountryCardData GetCountry(string name) {
            return data.Where(country => country.country == name).FirstOrDefault();
        }
    }
}
