using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace EQx.Game.CountryCards {
    public class CountryCardDatabase : MonoBehaviour {

        public static CountryCardDatabase instance = null;
        [SerializeField]
        public List<CountryCardData> data;

        // Start is called before the first frame update
        void Awake() {
            if (instance) {
                Destroy(gameObject);
            } else {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }
        }

        public CountryCardData GetCountry(int id) {
            return data.Where(country => country.cardID == id).FirstOrDefault();
        }

        public CountryCardData GetCountry(string name) {
            return data.Where(country => country.country == name).FirstOrDefault();
        }
    }
}
