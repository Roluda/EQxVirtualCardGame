using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Scenery {
    public class AudienceRing : MonoBehaviour {

        [SerializeField, Range(0, 100)]
        public float radius = 5;
        [SerializeField, Range(0, 100)]
        public float height = 1;
        [SerializeField, Range(1, 100)]
        public int vertices = 20;
        [SerializeField, Range(0, 360)]
        public float arc = 180;
        [SerializeField, Range(0, 90)]
        public float angleOffset = 0;
        [SerializeField]
        LineRenderer lineRenderer = default;

        [SerializeField]
        bool update = false;

        private void OnValidate() {
            if (update) {
                update = false;
                CalculatePoints();
            }
        }

        public void CalculatePoints() {
            if (!lineRenderer) {
                return;
            }
            var points = new Vector3[vertices];
            float angle = arc / vertices;
            for(int i= 0; i< vertices; i++) {
                points[i] = Vector3.up * height + Quaternion.Euler(0, i*-angle-angleOffset, 0) * Vector3.right * radius;
            }
            lineRenderer.positionCount = vertices;
            lineRenderer.SetPositions(points);
        }
    }
}
