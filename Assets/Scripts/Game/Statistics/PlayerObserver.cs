using EQx.Game.Player;
using EQx.Game.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EQx.Game.Statistics {
    public class PlayerObserver : MonoBehaviour {

        public static PlayerObserver instance;

        List<PlayerTrack> playerTracks = new List<PlayerTrack>();

        public void Register(CardPlayer player) {
            Debug.Log(name + ".Register: " + player);
            player.onCommited += CommitedListener;
            player.onPayedBlind += PayedBlindListener;

            string userID = player.photonView.Owner.UserId;
            var playerTrack = playerTracks.Where(track => track.userID == userID).FirstOrDefault();
            if (playerTrack == null) {
                playerTracks.Add(new PlayerTrack(player));
            } else {
                playerTrack.player = player;
            }
        }

        private void PayedBlindListener(CardPlayer player) {
        }

        private void CommitedListener(CardPlayer player) {
        }

        public void Unregister(CardPlayer player) {
            player.onCommited -= CommitedListener;
            player.onPayedBlind -= PayedBlindListener;
        }

        // Start is called before the first frame update
        void Awake() {
            if (instance != null) {
                Destroy(gameObject);
            } else {
                instance = this;
            }
        }

        // Update is called once per frame
        void Update() {

        }

        private void OnDestroy() {
            if(instance == this) {
                instance = null;
            }
        }
    }
}
