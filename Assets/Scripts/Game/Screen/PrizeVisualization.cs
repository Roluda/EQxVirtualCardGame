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
            
            foreach (var participant in RoundManager.instance.AllActiveParticipants()) {
                if(participant.state == RoundState.Won || participant.state == RoundState.Lost) {
                    entries[participant.player] = Instantiate(entryPrefab, spawnContext);
                    entries[participant.player].SetName(participant.player.playerName);
                    entries[participant.player].SetIcon(sprites[participant.player.avatarID]);
                    entries[participant.player].SetValueInstant(PlayerObserver.instance.GetCapital(participant.player));
                    capitals[participant.player] = PlayerObserver.instance.GetCapital(participant.player);
                }
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
            foreach(var participant in RoundManager.instance.AllActiveParticipants()) {
                if(participant.state == RoundState.Won || participant.state == RoundState.Lost) {
                    capitals[participant.player] = 
                        PlayerObserver.instance.GetCapital(participant.player) 
                        - PlayerObserver.instance.GetCommitment(participant.player)
                        + PlayerObserver.instance.GetWinnings(participant.player);
                    if (entries.ContainsKey(participant.player)) {
                        entries[participant.player].SetGain(0);
                    }
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
            capitals.Clear();
            infoText.text = "";
            foreach (var entry in entries) {
                Destroy(entry.Value.gameObject);
            }
            entries.Clear();
        }
    }
}
