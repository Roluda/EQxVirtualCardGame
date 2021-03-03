using EQx.Game.Investing;
using EQx.Game.Player;
using EQx.Game.Statistics;
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
        public int rank;

        private void CommitedListener(CardPlayer player) {
            Debug.Log(name + ".CommitedListener");
            if (player == observedPlayer) {
                vcpText.text = $"{vcpPrefix}{Math.Round(PlayerObserver.instance.GetVCP(player) * 100, 0)}%";
            }
        }


        private void CapitalUpdatedListener(CardPlayer player) {
            rank = InvestmentManager.instance.GetRank(observedPlayer);
            transform.SetSiblingIndex(rank-1);
            rankText.text = $"{rankPrefix}{rank}";
            if (player == observedPlayer) {
                int capital = InvestmentManager.instance.Capital(player);
                coinsText.text = $"{coinsPrefix}{capital}";
            }
        }

        public void Init(CardPlayer player) {
            observedPlayer = player;
            nameText.text = player.playerName;
            var sprites = Resources.LoadAll<Sprite>("Sprites/Characters");
            icon.sprite = sprites[player.avatarID];
        }

        private void Start() {
            InvestmentManager.instance.onCapitalUpdated += CapitalUpdatedListener;
            InvestmentManager.instance.onCommited += CommitedListener;
        }

        private void OnDestroy() {
            InvestmentManager.instance.onCapitalUpdated -= CapitalUpdatedListener;
            InvestmentManager.instance.onCommited -= CommitedListener;
        }
    }
}
