using TMPro;
using UnityEngine;

namespace EQx.Game.Player {
    public class PlayerName : MonoBehaviour {
        [SerializeField]
        TestPlayer testPlayer;
        [SerializeField]
        TextMeshPro playerName;

        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void LateUpdate() {
            playerName.text = testPlayer.networkObject.Owner.Name;
        }
    }
}
