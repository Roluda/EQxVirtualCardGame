using EQx.Game.Investing;
using EQx.Game.Player;
using EQx.Game.Statistics;
using EQx.Game.Table;
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
        TrackStat sortingMethod = TrackStat.Capital;

        public void OrderByVCP() {
            sortingMethod = TrackStat.VCP;
            rank = PlayerObserver.instance.GetRank(observedPlayer, TrackStat.VCP, RoundManager.instance.currentRound-1);
            UpdateRanking();
        }

        public void OrderByCapital() {
            sortingMethod = TrackStat.Capital;
            rank = PlayerObserver.instance.GetRank(observedPlayer, TrackStat.Capital, RoundManager.instance.currentRound);
            UpdateRanking();
        }

        void UpdateRanking() {
            transform.SetSiblingIndex(rank);
            rankText.text = $"{rankPrefix}{rank+1}";
        }

        private void UpdateValues() {
            int capital = InvestmentManager.instance.Capital(observedPlayer);
            coinsText.text = $"{capital}";
            vcpText.text = $"{Math.Round(PlayerObserver.instance.GetVCP(observedPlayer, RoundManager.instance.currentRound-1) * 100, 0)}%";
            switch (sortingMethod) {
                case TrackStat.Capital:
                    OrderByCapital();
                    break;
                case TrackStat.VCP:
                    OrderByVCP();
                    break;
                default:
                    throw new NotImplementedException("cant sort by this method");
            }
        }

        public void Init(CardPlayer player) {
            observedPlayer = player;
            nameText.text = player.playerName;
            var sprites = Resources.LoadAll<Sprite>("Sprites/Characters");
            icon.sprite = sprites[player.avatarID];
            RoundManager.instance.onNewRound += UpdateValues;
        }

        private void OnDestroy() {
            if (RoundManager.instance) {
                RoundManager.instance.onNewRound -= UpdateValues;
            }
        }
    }
}
