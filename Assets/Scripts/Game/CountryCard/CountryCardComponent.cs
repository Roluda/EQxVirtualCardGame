using System;
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
            observedCard.onTargetPositionSet += TargetPositionSetListener;
            observedCard.onTargetRotationSet += TargetRotationSetListener;
            observedCard.onCardDrawn += CardDrawnListener;
            observedCard.onCardRevealed += CardRevealedListener;
            observedCard.onNewCardData += NewCardDataListener;
            observedCard.onCardSelected += CardSelectedListener;
            observedCard.onCardUnselected += CardUnselectedListener;
            observedCard.onCardPlayed += CardPlayedListener;
            observedCard.onVariableHighlighted += VariableHighlightedListener;
            Enable();
        }

        private void OnDisable() {
            observedCard.onTargetPositionSet -= TargetPositionSetListener;
            observedCard.onCardDrawn -= CardDrawnListener;
            observedCard.onCardRevealed -= CardRevealedListener;
            observedCard.onNewCardData -= NewCardDataListener;
            observedCard.onCardSelected -= CardSelectedListener;
            observedCard.onCardUnselected -= CardUnselectedListener;
            observedCard.onCardPlayed -= CardPlayedListener;
            observedCard.onVariableHighlighted -= VariableHighlightedListener;
            Disable();
        }

        protected virtual void CardRevealedListener() { }
        protected virtual void CardDrawnListener() { }
        protected virtual void NewCardDataListener() { }
        protected virtual void CardPlayedListener() { }
        protected virtual void TargetRotationSetListener(Vector3 target) { }
        protected virtual void TargetPositionSetListener(Vector3 target) { }
        protected virtual void CardSelectedListener(CountryCard card) { }
        protected virtual void CardUnselectedListener(CountryCard card) { }
        protected virtual void VariableHighlightedListener(EQxVariable variable) { }

        protected virtual void Validate() { }
        protected virtual void Enable() { }
        protected virtual void Disable() { }
    }
}
