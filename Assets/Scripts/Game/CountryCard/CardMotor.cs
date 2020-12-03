using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.CountryCards {
    public class CardMotor : CountryCardComponent {

        [SerializeField, Range(0, 100)]
        float movementSpeed = 5;


        public Vector3 targetPosition;
        public Vector3 targetLookDirection;
        public Vector3 targetUpDirection;

        private void Update() {
            Vector3 movementDirection = Vector3.ClampMagnitude(targetPosition - observedCard.transform.position, 1);
            transform.Translate(movementDirection * movementSpeed * Time.deltaTime);

            Vector3 switchedZDirection = new Vector3(targetLookDirection.x, targetLookDirection.y, -targetLookDirection.z);

            Quaternion rotation = Quaternion.LookRotation(switchedZDirection, targetUpDirection);
            transform.rotation = rotation;
        }
    }
}
