using EQx.Game.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Player {
    public class LocalPlayerAudio : MonoBehaviour {

        [SerializeField]
        RandomSFX clapsSFX;

        private void Awake() {
            CardPlayer.localPlayerReady += Initialize;
        }

        void Claps() {
            AudioManager.instance.PlayOneShot(clapsSFX);
        }

        void Initialize(CardPlayer player) {
            CardPlayer.localPlayerReady -= Initialize;
            player.onPlacedCard += (cp, id) => Claps();
            player.onInvestedCoins += (cp, amount) => Claps();
        }
    }
}
