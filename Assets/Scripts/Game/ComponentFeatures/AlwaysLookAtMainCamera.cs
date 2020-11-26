using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.ComponentFeature {
    public class AlwaysLookAtMainCamera : MonoBehaviour {
        [SerializeField]
        bool lookAway = true;

        // Update is called once per frame
        void LateUpdate() {
            transform.LookAt(Camera.main.transform);
        }
    }
}
