using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EQx.Game.Player;
using System.Linq;
using EQx.Game.Table;
using EQx.Game.Audio;

namespace EQx.Game.Screen {
    public class WinnerVisualization : MonoBehaviour {

        [SerializeField]
        Transform spawnContext = default;
        [SerializeField]
        WinnerEntry winnerEntry = default;
        [SerializeField]
        float baseValuePresentationTime;
        [SerializeField]
        RandomSFX winApplause = default;
        [SerializeField]
        RandomSFX winDang = default;
        [SerializeField]
        RandomSFX drumRoll = default;

        List<WinnerEntry> candidates = new List<WinnerEntry>();

        public void SpawnBars() {
            float highestValue = 0;
            foreach (var player in RoundManager.instance.registeredPlayers) {
                if (player.state == PlayerState.Unregistered) {
                    continue;
                }
                var candidate = Instantiate(winnerEntry, spawnContext);
                candidate.Initialize(player);
                candidates.Add(candidate);
                highestValue = player.combinedValue > highestValue ? player.combinedValue : highestValue;
            }
            foreach(var candidate in candidates) {
                candidate.presentSpeed = highestValue / baseValuePresentationTime;
            }
        }

        public void PresentBaseValues() {
            drumRoll.Play();
            foreach (var candidate in candidates) {
                candidate.PresentBaseValues();
            }
        }

        public void PresentCombinedValues() {
            drumRoll.Play();
            foreach (var candidate in candidates) {
                candidate.PresentCombinedValues();
            }
        }

        public void PresentBonusValues() {
            foreach (var candidate in candidates) {
                candidate.PresentBonusValues();
            }
        }

        public void ApplyBonusValues() {
            foreach (var candidate in candidates) {
                candidate.ApplyBonusValues();
            }
        }

        public void HighlightWinner() {
            foreach (var candidate in candidates) {
                candidate.Win();
            }
            winApplause.Play();
            winDang.Play();
        }

        public void CleanUp() {
            foreach (var candidate in candidates) {
                candidate.Clear();
            }
            candidates.Clear();
        }
    }
}
