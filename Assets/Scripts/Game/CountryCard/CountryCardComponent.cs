using UnityEngine;

namespace EQx.Game.CountryCards {
    public class CountryCardComponent : MonoBehaviour {

        [SerializeField]
        public CountryCard observedCard = default;

        private void OnValidate() {
            var card = GetComponentInParent<CountryCard>();
            if (card) {
                observedCard = card;
            }
            Validate();
        }

        private void OnEnable() {
            var card = GetComponentInParent<CountryCard>();
            if (card) {
                observedCard = card;
            }
            observedCard.onNewCardData += NewCardDataListener;
            observedCard.onCardSelected += CardSelectedListener;
            observedCard.onCardUnselected += CardUnselectedListener;
            observedCard.onCardPlayed += CardPlayedListener;
            Enable();
        }

        private void OnDisable() {
            observedCard.onNewCardData -= NewCardDataListener;
            observedCard.onCardSelected -= CardSelectedListener;
            observedCard.onCardUnselected -= CardUnselectedListener;
            observedCard.onCardPlayed -= CardPlayedListener;
            Disable();
        }

        protected virtual void NewCardDataListener() { }
        protected virtual void CardSelectedListener(CountryCard card) { }
        protected virtual void CardUnselectedListener(CountryCard card) { }
        protected virtual void CardPlayedListener() { }

        protected virtual void Validate() { }
        protected virtual void Enable() { }
        protected virtual void Disable() { }
    }
}
