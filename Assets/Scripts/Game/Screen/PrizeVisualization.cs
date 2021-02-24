using EQx.Game.Investing;
using EQx.Game.Player;
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
        PrizeEntry entryPrefab;

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
                entries[player].SetValueInstant(InvestmentManager.instance.Capital(player) + InvestmentManager.instance.LastCommitment(player));
                capitals[player] = InvestmentManager.instance.Capital(player) + InvestmentManager.instance.LastCommitment(player);
            }
            SetRanks();
        }

        public void ShowGains() {
            foreach(var entry in entries) {
                int commitment = InvestmentManager.instance.LastCommitment(entry.Key);
                if (entry.Key == InvestmentManager.instance.prizeWinner) {
                    int prize = InvestmentManager.instance.prize;
                    entry.Value.SetGain(prize - commitment);
                    entry.Value.SetValueLerp(InvestmentManager.instance.Capital(entry.Key) + prize);
                } else {
                    entry.Value.SetGain(-commitment);
                    entry.Value.SetValueLerp(InvestmentManager.instance.Capital(entry.Key));
                }
            }
        }

        public void UpdateRankings() {
            capitals.Clear();
            foreach(var player in RoundManager.instance.registeredPlayers) {
                int prize = InvestmentManager.instance.prizeWinner == player ? InvestmentManager.instance.prize : 0;
                capitals[player] = InvestmentManager.instance.Capital(player) + prize;
                entries[player].SetGain(0);
            }
            SetRanks();
        }

        public void Hide() {
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
            foreach(var entry in entries) {
                Destroy(entry.Value.gameObject);
            }
            entries.Clear();
        }
    }
}
