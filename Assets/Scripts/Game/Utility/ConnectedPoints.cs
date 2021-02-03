using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Utility {
    public class ConnectedPoints : MonoBehaviour {
        [SerializeField]
        public Transform pointA = default;
        [SerializeField]
        public Transform pointB = default;
        [SerializeField]
        LineRenderer lineRenderer = default;

        [SerializeField]
        bool connect = false;

        private void OnValidate() {
            if (connect) {
                connect = false;
                CalculatePoints();
            }
        }

        // Update is called once per frame
        void Update() {
            CalculatePoints();
        }

        void CalculatePoints() {
            Vector3 middlePoint = new Vector3(pointA.position.x, pointB.position.y, pointA.position.z);
            Vector3[] points = new Vector3[] { pointA.position, middlePoint, pointB.position };
            lineRenderer.positionCount = 3;
            lineRenderer.SetPositions(points);
        }
    }
}
