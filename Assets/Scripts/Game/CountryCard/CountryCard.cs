using UnityEngine;
using UnityEngine.Events;

namespace EQx.Game.CountryCards {
    public class CountryCard : MonoBehaviour {

        public UnityAction onNewCardData;
        public UnityAction onCardPlayed;
        public UnityAction onCardRevealed;
        public UnityAction onCardDrawn;
        public UnityAction<Vector3> onTargetPositionSet;
        public UnityAction<Vector3> onTargetRotationSet;
        public UnityAction<CountryCard> onCardSelected;
        public UnityAction<CountryCard> onCardUnselected;
        public UnityAction<CountryCard> onCardAffordable;
        public UnityAction<CountryCard> onCardUnaffordable;
        public UnityAction<EQxVariableType> onVariableHighlighted;

        [SerializeField]
        Canvas frontCanvas = default;
        [SerializeField]
        Canvas backCanvas = default;

        [SerializeField]
        EQxCountryData dataCache = default;
        public EQxCountryData data {
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
        public int id { get => CountryCardDatabase.instance.GetIndex(data); }

        [SerializeField]
        int layerCache;
        public int layer {
            get => layerCache;
            set {
                frontCanvas.sortingOrder = value;
                backCanvas.sortingOrder = value;
                layerCache = value;
            }
        }

        public void PlayCard() {
            onCardPlayed?.Invoke();
        }

        public void RevealCard() {
            onCardRevealed?.Invoke();
        }

        public void DrawCard() {
            onCardDrawn?.Invoke();
        }

        public void HighlightVariabe(EQxVariableType variable) {
            onVariableHighlighted?.Invoke(variable);
        }

        public void SetTargetPosition(Vector3 target) {
            onTargetPositionSet?.Invoke(target);
        }

        public void SetTargetRotation(Vector3 target) {
            onTargetRotationSet?.Invoke(target);
        }

        bool affordableCache = false;
        public bool affordable {
            get => affordableCache;
            set {
                if (value == affordableCache) {
                    return;
                }
                affordableCache = value;
                if (value) {
                    onCardAffordable?.Invoke(this);
                } else {
                    onCardUnaffordable?.Invoke(this);
                }
            }
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
    }
}
