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
        RandomSFX winApplause = default;
        [SerializeField]
        RandomSFX winDang = default;
        [SerializeField]
        RandomSFX drumRoll = default;

        List<WinnerEntry> candidates = new List<WinnerEntry>();

        public void SpawnBars() {
            float targetValue = RoundManager.instance.AllActiveParticipants().Max(part => part.combinedValue);
            foreach (var participant in RoundManager.instance.AllActiveParticipants()) {
                if (participant.state == RoundState.Won || participant.state == RoundState.Lost) {
                    var candidate = Instantiate(winnerEntry, spawnContext);
                    candidate.Initialize(participant.player, 0, targetValue, presentationTime);
                    candidates.Add(candidate);
                }
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
