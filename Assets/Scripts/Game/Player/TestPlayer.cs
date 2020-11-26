using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

namespace EQx.Game.Player {
    public class TestPlayer : TestPlayerBehavior {

        [SerializeField]
        Rigidbody attachedRigidbody = default;
        [SerializeField]
        float forwardVelocity = 5;
        [SerializeField]
        float jumpForce = 3;
        [SerializeField]
        float rotationVelocity = 20;

        public override void AssignColor(RpcArgs args) {
            MainThreadManager.Run(() => {
                GetComponent<MeshRenderer>().material.SetColor("_BaseColor",args.GetNext<Color>());
            });
        }

        public override void AssignName(RpcArgs args) {
            MainThreadManager.Run(() => {
                networkObject.Owner.Name = args.GetNext<string>();
            });
        }

        public override void jump(RpcArgs args) {
            MainThreadManager.Run(() => {
                attachedRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            });
        }

        protected override void NetworkStart() {
            base.NetworkStart();
            if (networkObject.IsOwner) {
                networkObject.SendRpc(RPC_ASSIGN_NAME, Receivers.AllBuffered, PlayerPrefs.GetString(PlayerPrefKeys.PLAYERNAME, "Anonymous"));
                networkObject.SendRpc(RPC_ASSIGN_COLOR, Receivers.AllBuffered, Random.ColorHSV(0,1,.8f,.8f,1,1));
            }
        }

        // Update is called once per frame
        void Update() {
            if (networkObject == null) {
                return;
            }

            if (!networkObject.IsOwner) {
                transform.position = networkObject.position;
                transform.rotation = networkObject.rotation;
                return;
            }

            if (Input.GetMouseButton(0)) {
                attachedRigidbody.AddForce(-transform.forward * forwardVelocity);
            }

            if (Input.GetMouseButton(1)) {
                transform.Rotate(Vector3.up * rotationVelocity * Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                networkObject.SendRpc(RPC_JUMP, Receivers.All);
            }

            if (networkObject.IsOwner) {
                networkObject.rotation = transform.rotation;
                networkObject.position = transform.position;
            }
        }
    }
}
