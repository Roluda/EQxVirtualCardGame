using EQx.Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Statistics {
    public class PlayerTrack {
        public PlayerTrack(CardPlayer player) {
            userID = player.photonView.Owner.UserId;
            this.player = player;
        }
        public CardPlayer player;
        public string userID;
        public Dictionary<int, int> commitments;
        public Dictionary<int, int> capital;
        public Dictionary<int, bool> won;
        public Dictionary<int, int> cardPlaced;

    }
}
