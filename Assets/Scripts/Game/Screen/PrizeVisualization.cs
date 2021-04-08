using EQx.Game.Investing;
using EQx.Game.Player;
using EQx.Game.Statistics;
using EQx.Game.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace EQx.Game.Screen {
    public class PrizeVisualization : MonoBehaviour {

        [SerializeField]
        Transform spawnContext = default;

        [SerializeField]
        PrizeEntry entryPrefab = default;
        [SerializeField]
        TMP_Text infoText = default;
        [SerializeField]
        string commitmentText = "Elite Commitment";
        [SerializeField]
        string prizeText = "Jackpot";

        Dictionary<CardPlayer, PrizeEntry> entries = new Dictionary<CardPlayer, PrizeEntry>();

        Dictionary<CardPlayer, int> capitals = new Dictionary<CardPlayer, int>();



        public void SpawnEntries() {
            CleanUp();
            var sprites = Resources.LoadAll<Sprite>("Sprites/Characters");
            
            foreach (var player in RoundManager.instance.registeredPlayers) {
                if(player.state == PlayerState.Unregistered) {
                    continue;
                }
                entries[player] = Instantiate(entryPrefab, spawnContext);
                entries[player].SetName(player.playerName);
                entries[player].SetIcon(sprites[player.avatarID]);
                entries[player].SetValueInstant(PlayerObserver.instance.GetCapital(player));
                capitals[player] = PlayerObserver.instance.GetCapital(player);
            }
            SetRanks();
        }

        public void ShowCommitment() {
            infoText.text = commitmentText;
            foreach (var entry in entries) {
                entry.Value.SetValueLerp(PlayerObserver.instance.GetCapital(entry.Key) - PlayerObserver.instance.GetCommitment(entry.Key));
                entry.Value.SetCommitment(-PlayerObserver.instance.GetCommitment(entry.Key));
            }
        }

        public void ShowGains() {
            infoText.text = prizeText;
            foreach (var entry in entries) {
                entry.Value.SetValueLerp(PlayerObserver.instance.GetCapital(entry.Key) - PlayerObserver.instance.GetCommitment(entry.Key) + PlayerObserver.instance.GetWinnings(entry.Key));
                entry.Value.SetGain(PlayerObserver.instance.GetWinnings(entry.Key));
            }
        }

        public void UpdateRankings() {
            capitals.Clear();
            foreach(var player in RoundManager.instance.registeredPlayers) {
                capitals[player] = PlayerObserver.instance.GetCapital(player) - PlayerObserver.instance.GetCommitment(player) + PlayerObserver.instance.GetWinnings(player);
                if (entries.ContainsKey(player)) {
                    entries[player].SetGain(0);
                }
            }
            SetRanks();
        }

        public void Hide() {
            infoText.text= "";
            foreach(var entry in entries) {
                entry.Value.gameObject.SetActive(false);
            }
        }

        public void Show() {
            foreach (var entry in entries) {
                entry.Value.gameObject.SetActive(true);
            }
        }

        void SetRanks() {
            var ranking = capitals.ToList();
            ranking.Sort((x, y) => y.Value.CompareTo(x.Value));
            int rank = 1;
            foreach (var player in ranking) {
                entries[player.Key].SetRank(rank);
                rank++;
            }
            foreach(var entry in entries) {
                entry.Value.gameObject.transform.SetSiblingIndex(entry.Value.rank);
            }
        }

        public void CleanUp() {
            infoText.text = "";
            foreach (var entry in entries) {
                Destroy(entry.Value.gameObject);
            }
            entries.Clear();
        }
    }
}
