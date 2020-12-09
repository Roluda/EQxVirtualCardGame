using UnityEngine;
using UnityEngine.Events;

namespace EQx.Game.CountryCards {
    public class CountryCard : MonoBehaviour {

        public UnityAction onNewCardData;
        public UnityAction onCardPlayed;
        public UnityAction<CountryCard> onCardSelected;
        public UnityAction<CountryCard> onCardUnselected;

        [SerializeField]
        public CardMotor motor = default;
        [SerializeField]
        public Canvas cardCanvas;

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

        public void PlayCard() {
            onCardPlayed?.Invoke();
        }

        bool selectedCache = false;
        public bool selected {
            get => selectedCache;
            set {
                if(value == selectedCache) {
                    return;
                }
                selectedCache = value;
                if (value) {
                    onCardSelected?.Invoke(this);
                } else {
                    onCardUnselected?.Invoke(this);
                }
            }
        }

        private void Start() {
            onNewCardData?.Invoke();
        }

        private void OnMouseDown() {
            selected = true;
        }

        private void OnMouseUp() {
            selected = false;
        }
    }
}
