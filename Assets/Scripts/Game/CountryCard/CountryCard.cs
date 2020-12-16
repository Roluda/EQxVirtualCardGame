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
        public UnityAction<EQxVariable> onVariableHighlighted;

        [SerializeField]
        Canvas frontCanvas = default;
        [SerializeField]
        Canvas backCanvas = default;

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

        public void HighlightVariabe(EQxVariable variable) {
            onVariableHighlighted?.Invoke(variable);
        }

        public void SetTargetPosition(Vector3 target) {
            onTargetPositionSet?.Invoke(target);
        }

        public void SetTargetRotation(Vector3 target) {
            onTargetRotationSet?.Invoke(target);
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
