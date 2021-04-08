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
        float presentationTime = 5;
        [SerializeField]
        float extractRoundStartValue = 100;
        [SerializeField]
        float creationRoundStartValue = 0;
        [SerializeField]
        RandomSFX winApplause = default;
        [SerializeField]
        RandomSFX loseGlory = default;
        [SerializeField]
        RandomSFX winDang = default;
        [SerializeField]
        RandomSFX drumRoll = default;

        List<WinnerEntry> candidates = new List<WinnerEntry>();

        public void SpawnBars() {
            float targetValue = RoundManager.instance.winner.combinedValue;
            float startValue = RoundManager.instance.extractionRound ? extractRoundStartValue : creationRoundStartValue;
            foreach (var player in RoundManager.instance.registeredPlayers) {
                if (player.state == PlayerState.Unregistered) {
                    continue;
                }
                var candidate = Instantiate(winnerEntry, spawnContext);
                candidate.Initialize(player, startValue, targetValue, presentationTime);
                candidates.Add(candidate);
            }
        }

        public void PresentValues() {
            drumRoll.Play();
            foreach (var candidate in candidates) {
                candidate.PresentValue();
            }
        }

        public void HighlightWinner() {
            foreach (var candidate in candidates) {
                candidate.Win();
            }
            if (RoundManager.instance.extractionRound) {
                loseGlory.Play();
            } else {
                winApplause.Play();
            }
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
