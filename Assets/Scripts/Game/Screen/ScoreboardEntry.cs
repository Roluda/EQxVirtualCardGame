using EQx.Game.Investing;
using EQx.Game.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EQx.Game.Screen {
    public class ScoreboardEntry : MonoBehaviour {

        [SerializeField]
        Image icon = default;
        [SerializeField]
        TMP_Text nameText = default;
        [SerializeField]
        TMP_Text coinsText = default;
        [SerializeField]
        TMP_Text rankText = default;
        [SerializeField]
        TMP_Text vcpText = default;
        [SerializeField]
        string coinsPrefix = default;
        [SerializeField]
        string rankPrefix = default;
        [SerializeField]
        string vcpPrefix = default;

        public CardPlayer observedPlayer;


        private void OnEnable() {
            InvestmentManager.instance.onCapitalUpdated += CapitalUpdatedListener;
        }

        private void OnDisable() {
            InvestmentManager.instance.onCapitalUpdated -= CapitalUpdatedListener;
        }

        private void CapitalUpdatedListener(CardPlayer player) {
            if (player == observedPlayer) {
                int capital = InvestmentManager.instance.Capital(player);
                int rank = InvestmentManager.instance.GetRank(player);
                coinsText.text = $"{coinsPrefix}{capital}";
                rankText.text = $"{rankPrefix}{rank}";
            }
        }

        public void Init(CardPlayer player) {
            observedPlayer = player;
            nameText.text = player.playerName;
            var sprites = Resources.LoadAll<Sprite>("Sprites/Characters");
            icon.sprite = sprites[player.avatarID];
        }
    }
}
