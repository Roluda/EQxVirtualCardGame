using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Photon.Realtime;
using System.Collections.Generic;
using System;

namespace EQx.Menu {
    public class TableOption : MonoBehaviour {
        [SerializeField]
        TMP_Text nameText = default;
        [SerializeField]
        TMP_Text playersText = default;
        [SerializeField]
        Button connectButton = default;
        [SerializeField]
        TMP_Text roundsText = default;
        [SerializeField]
        string playersInfo = " Elites playing: ";
        [SerializeField]
        string roundsInfo = " Rounds played";
        public void SetData(RoomInfo info, UnityAction callback) {
            nameText.text = info.Name;
            var names = (string[])info.CustomProperties[TableBrowser.CONNECTED_PLAYERS];
            playersText.text = $"{info.PlayerCount}/{info.MaxPlayers}{playersInfo}{string.Join(", ", names)}";

            roundsText.text = $"{info.CustomProperties[TableBrowser.CURRENT_ROUND]}/{info.CustomProperties[TableBrowser.MAX_ROUNDS]}{roundsInfo}";

            connectButton.onClick.AddListener(callback);
        }
    }
}
