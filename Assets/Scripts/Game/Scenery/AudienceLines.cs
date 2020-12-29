using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EQx.Game.Scenery {
    public class AudienceLines : MonoBehaviour {

        [SerializeField, Range(0, 30)]
        int ringCount = 10;
        [SerializeField]
        float radiusIncrement = 2;
        [SerializeField]
        float heightIncrement = 1;
        [SerializeField]
        float baseRadius = 10;
        [SerializeField]
        float baseHeight = 1;
        [SerializeField]
        float angleOffsetInterLines = 5;
        [SerializeField]
        int moduloOffset = 2;
        [SerializeField]
        AudienceRing ringPrefab = default;

        List<AudienceRing> rings = new List<AudienceRing>();

        private void OnValidate() {
            UpdateRings();
        }

        public void UpdateRings() {
            for (int i = 0; i < rings.Count; i++) {
                rings[i].radius = baseRadius + i * radiusIncrement;
                rings[i].height = baseHeight + i * heightIncrement;
                rings[i].angleOffset = angleOffsetInterLines * (i % moduloOffset);
                rings[i].CalculatePoints();
            }
        }

        public void CalculateRings() {
            foreach(var ring in rings) {
                if (ring) {
                    DestroyImmediate(ring.gameObject);
                }
            }
            rings = new List<AudienceRing>();
            for(int i=0; i<ringCount; i++) {
                rings.Add(PrefabUtility.InstantiatePrefab(ringPrefab, transform) as AudienceRing);
            }
            UpdateRings();
        }
    }
}
