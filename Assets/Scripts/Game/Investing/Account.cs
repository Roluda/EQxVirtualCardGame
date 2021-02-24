using EQx.Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Investing {
    public class Account {
        public Account(CardPlayer player) {
            this.player = player;
            userID = player.photonView.Owner.UserId;
        }
        public bool isActive = true;
        public CardPlayer player;
        public string userID = "";
        public int capital = 0;
        public int investment = 0;
        public int payedBlind = 0;
        public int lastCommitment = 0;
        public int commitment => payedBlind + investment;

        public int TakeCommitment() {
            lastCommitment = commitment;
            payedBlind = 0;
            investment = 0;
            return lastCommitment;
        }
    }
}
