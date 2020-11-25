using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Player {
    public class TestPlayer : TestPlayerBehavior {

        [SerializeField]
        Rigidbody attachedRigidbody = default;
        [SerializeField]
        float forwardVelocity = 5;
        [SerializeField]
        float backwardVelocity = 3;
        [SerializeField]
        float rotationVelocity = 20;

        public override void jump(RpcArgs args) {
            MainThreadManager.Run(() => {
                transform.Rotate(new Vector3(0, 20, 0));
            });
        }

        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            // Unity's Update() running, before this object is instantiated
            // on the network is *very* rare, but better be safe 100%
            if (networkObject == null) {
                return;
            }

            if (!networkObject.IsOwner) {
                transform.position = networkObject.position;
                transform.rotation = networkObject.rotation;
                return;
            }

            attachedRigidbody.velocity = CalculateVelocity();

            if (Input.GetKeyDown(KeyCode.Space)) {
                networkObject.SendRpc(RPC_JUMP, Receivers.All);
            }

            if (networkObject.IsOwner) {
                networkObject.rotation = transform.rotation;
                networkObject.position = transform.position;
            }
        }

        Vector3 CalculateVelocity() {
            Vector3 resultingVelocity = Vector3.zero;
            if (Input.GetMouseButton(0)) {
                resultingVelocity += transform.forward * forwardVelocity;
            }
            if (Input.GetMouseButton(1)) {
                resultingVelocity += -transform.forward * backwardVelocity;
            }
            return resultingVelocity;
        }
    }
}
