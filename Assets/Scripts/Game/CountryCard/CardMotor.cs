using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.CountryCards {
    public class CardMotor : CountryCardComponent {

        [SerializeField, Range(0, 100)]
        public float movementSpeed = 5;

        [SerializeField, Range(0, 360)]
        float flipSpeed = 90;
        [SerializeField]
        bool moveWhileSelected;
        [SerializeField]
        bool rotateWhileSelected;


        [SerializeField]
        Vector3 targetRotation;

        [SerializeField]
        Vector3 targetPosition;

        protected override void TargetRotationSetListener(Vector3 target) {
            targetRotation = target;
        }

        protected override void TargetPositionSetListener(Vector3 target) {
            targetPosition = target;
        }

        private void Update() {
            if (moveWhileSelected || !observedCard.selected) {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
            }
            if (rotateWhileSelected || !observedCard.selected) {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), flipSpeed * Time.deltaTime);
            }
        }
    }
}
