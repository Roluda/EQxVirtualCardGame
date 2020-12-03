using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EQx.Game.CountryCards {
    public class DragOnSelect : CountryCardComponent {
        // Update is called once per frame
        [SerializeField]
        int sortingOrder = 10;
        [SerializeField]
        Vector3 normal = Vector3.forward;

        Plane movingPlane = default;

        protected override void CardSelectedListener(CountryCard card) {
            movingPlane = new Plane(normal, card.transform.position);
        }

        void Update() {
            if (observedCard.selected) {
                observedCard.cardCanvas.sortingOrder = sortingOrder;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                movingPlane.Raycast(ray, out float enter);
                observedCard.transform.position = ray.GetPoint(enter);
            }
        }
    }
}
