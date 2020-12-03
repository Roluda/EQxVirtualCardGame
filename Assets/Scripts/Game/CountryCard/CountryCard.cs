using UnityEngine;

namespace EQx.Game.CountryCards {
    public class CountryCard : MonoBehaviour {

        public delegate void NewCardData();
        public event NewCardData onNewCardData;
        public delegate void CardPlayed();
        public event CardPlayed onCardPlayed;
        public delegate void CardSelected();
        public event CardSelected onCardSelected;
        public delegate void CardUnselected();
        public event CardUnselected onCardUnselected;

        [SerializeField]
        CountryCardData dataCache = default;
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

        private void OnValidate() {
            onNewCardData?.Invoke();
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

        private void Start() {
            onNewCardData?.Invoke();
        }
    }
}
