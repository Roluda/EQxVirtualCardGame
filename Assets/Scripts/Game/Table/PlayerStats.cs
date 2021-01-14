using EQx.Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Table {
    public class PlayerStats {
        public CardPlayer player;
        public int placedCard;
        public float baseValue;
        public float bonusValue;
        public float combinedValue => baseValue + bonusValue;
        public bool won;
    }
}
