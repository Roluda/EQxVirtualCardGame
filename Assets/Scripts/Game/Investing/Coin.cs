using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Investing {
    public class Coin : MonoBehaviour {
        [SerializeField]
        Rigidbody attachedRigidbody = default;
        [SerializeField]
        BoxCollider attachedCollider = default;

        public float height => attachedCollider.size.y * transform.localScale.y;
        public float radius=> attachedCollider.size.x / 2 * transform.localScale.x;
        public bool isKinematic => attachedRigidbody.isKinematic;

        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }
    }
}
