using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Player {
    public class TestPlayer : TestPlayerBehavior {

        public override void jump(RpcArgs args) {
            throw new System.NotImplementedException();
        }

        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            if (networkObject.IsOwner) {
                networkObject.rotation = transform.rotation;
                networkObject.position = transform.position;
            }
        }
    }
}
