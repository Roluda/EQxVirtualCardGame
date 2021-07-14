using EQx.Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Table {
    public class CardInventory {
        public List<int> cards = new List<int>();
        public CardPlayer owner = default;
        public string ownerID = default;
    }
}
