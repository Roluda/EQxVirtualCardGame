using EQx.Game.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EQx.Game.CountryCards {
    public class DragAndSelectOnClick : CountryCardComponent {
        // Update is called once per frame
        [SerializeField]
        Collider attachedCollider = default;
        [SerializeField]
        int sortingOrder = 10;
        [SerializeField]
        int targetLayer = 6;
        [SerializeField]
        Vector3 normal = Vector3.forward;

        Plane movingPlane = default;

        int previousLayer;
        int previousOrder;

        private void OnValidate() {
            if (!attachedCollider) {
                attachedCollider = GetComponent<Collider>();
            }
        }

        void Update() {
            if (observedCard.selected) { 
                observedCard.order = sortingOrder;
                observedCard.layer = targetLayer;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                movingPlane.Raycast(ray, out float enter);
                observedCard.transform.position = ray.GetPoint(enter);
            }
        }

        void OnMouseDown() {
            previousLayer = observedCard.layer;
            previousOrder = observedCard.order;
            observedCard.selected = true;
            movingPlane = new Plane(normal, observedCard.transform.position);
        }

        void OnMouseUp() {
            observedCard.layer = previousLayer;
            observedCard.order = previousOrder;
            observedCard.selected = false;
        }
    }
}
