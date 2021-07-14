using EQx.Game.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EQx.Game.Table {
    public class RoundParticipant {
        public CardPlayer player;
        public string userID;
        public bool active = false;
        public int seatNumber = -1;

        public int placedCardID = -1;
        public int actorNumber => player.photonView.Owner.ActorNumber;

        public float baseValue = 0;
        public float bonusValue = 0;
        public float combinedValue => baseValue + bonusValue;

        public RoundState state = RoundState.Start;

        public void Reset() {
            placedCardID = -1;
            baseValue = 0;
            bonusValue = 0;
        }
    }
}
