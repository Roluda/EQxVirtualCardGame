﻿using EQx.Game.Investing;
using EQx.Game.Player;
using EQx.Game.Statistics;
using EQx.Game.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EQx.Game.Screen {
    public class Scoreboard : MonoBehaviour {
        [SerializeField]
        Transform context = default;
        [SerializeField]
        ScoreboardEntry entryPrefab = default;

        List<ScoreboardEntry> entries = new List<ScoreboardEntry>();


        private void RemoveEntry(CardPlayer player) {
            var removedEntry = entries.Where(entry => entry.observedPlayer == player).First();
            entries.Remove(removedEntry);
            Destroy(removedEntry.gameObject);
        }

        private void AddEntry(CardPlayer player) {
            var entry = Instantiate(entryPrefab, context);
            entry.Init(player);
            entries.Add(entry);
            OrderByCapital();
            UpdateEntries();
        }

        public void UpdateEntries() {
            entries.ForEach(e => e.UpdateValues());
            entries.ForEach(e => e.UpdateRanking());
        }

        public void OrderByCapital() {
            entries.ForEach(entry => entry.OrderByCapital());
            UpdateEntries();
        }

        public void OrderByVCP() {
            entries.ForEach(entry => entry.OrderByVCP());
            UpdateEntries();
        }

        // Start is called before the first frame update
        void Start() {
            RoundManager.instance.onPlayerRegister += AddEntry;
            RoundManager.instance.onPlayerUnregister += RemoveEntry;
        }
    }
}
