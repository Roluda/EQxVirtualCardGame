using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EQx.Game.CountryCards;


namespace EQx.Game {
    public class GameManager : MonoBehaviour {

        public static GameManager instance;

        [SerializeField]
        public CountryCardDatabase countryCardDatabase;
        // Start is called before the first frame update
        void Start() {
            if (instance) {
                Destroy(gameObject);
            } else {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
