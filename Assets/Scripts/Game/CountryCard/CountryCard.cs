using UnityEngine;

namespace EQx.Game.CountryCards {
    public class CountryCard : MonoBehaviour {

        [SerializeField]
        bool randomValue = false;

        public delegate void NewCardData();
        public event NewCardData onNewCardData;
        public delegate void CardPlayed();
        public event CardPlayed onCardPlayed;
        public delegate void CardSelected();
        public event CardSelected onCardSelected;
        public delegate void CardUnselected();
        public event CardUnselected onCardUnselected;

        private void OnValidate() {
            if (randomValue) {
                randomValue = false;
                float[] randomValues = new float[20];
                for (int i=0; i<randomValues.Length; i++) {
                    randomValues[i] = Random.value * 100;
                }
                data = new CountryCardData("Country Name", randomValues);
            }
        }

        private void Start() {
            onNewCardData?.Invoke();
        }

        CountryCardData dataCache = new CountryCardData("Country Name", new float[20]);
        public CountryCardData data {
            get {
                return dataCache;
            }
            set {
                if (value != dataCache) {
                    dataCache = value;
                    onNewCardData?.Invoke();
                }
            }
        }

        bool selectedCache = false;
        public bool selected {
            get => selected;
            set {
                if(value == selectedCache) {
                    return;
                }
                selectedCache = value;
                if (value) {
                    onCardSelected?.Invoke();
                } else {
                    onCardUnselected?.Invoke();
                }
            }
        }
    }
}
