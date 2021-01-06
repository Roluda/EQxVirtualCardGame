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
        public int extraction = 0;
        public int payedBlind = 0;
        public int commitment => payedBlind + investment - extraction;

        public bool AbleToPay(int amount) {
            return capital - amount >= 0;
        }

        public bool SufficientBlind(int request) {
            return payedBlind >= request;
        }

        public bool CanExtract(int amount) {
            return payedBlind - amount >= 0;
        }

        public int TakeCommitment() {
            int com = commitment;
            payedBlind = 0;
            extraction = 0;
            investment = 0;
            return com;
        }
    }
}
