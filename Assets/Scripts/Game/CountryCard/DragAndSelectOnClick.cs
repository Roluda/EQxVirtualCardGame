using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EQx.Game.CountryCards {
    public class DragAndSelectOnClick : CountryCardComponent {
        // Update is called once per frame
        [SerializeField]
        int sortingOrder = 10;
        [SerializeField]
        Vector3 normal = Vector3.forward;

        Plane movingPlane = default;

        int previousLayer;

        void Update() {
            if (observedCard.selected) { 
                observedCard.layer = sortingOrder;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                movingPlane.Raycast(ray, out float enter);
                observedCard.transform.position = ray.GetPoint(enter);
            }
        }

        private void OnMouseDown() {
            previousLayer = observedCard.layer;
            observedCard.selected = true;
            movingPlane = new Plane(normal, observedCard.transform.position);
        }

        private void OnMouseUp() {
            observedCard.layer = previousLayer;
            observedCard.selected = false;
        }
    }
}
