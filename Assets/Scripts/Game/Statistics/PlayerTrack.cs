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
        public Dictionary<int, int> winnings = new Dictionary<int, int>();
        public Dictionary<int, int> commitments = new Dictionary<int, int>();
        public Dictionary<int, int> investments = new Dictionary<int, int>();
        public Dictionary<int, int> capital = new Dictionary<int, int>();
        public Dictionary<int, bool> won = new Dictionary<int, bool>();
        public Dictionary<int, int> cardPlaced = new Dictionary<int, int>();
        public Dictionary<int, float> valueCreationPercentile = new Dictionary<int, float>();

    }
}
