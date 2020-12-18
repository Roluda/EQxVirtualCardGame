using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EQx.Game.Player;
using System.Linq;

namespace EQx.Game.Table {
    public class WinnerVisualization : MonoBehaviour {

        [SerializeField]
        Transform spawnContext = default;
        [SerializeField]
        WinnerAvatar winnerAvatarPrefab = default;

        List<WinnerAvatar> candidates = new List<WinnerAvatar>();

        public void SpawnWinners() {
            foreach (var placedCard in RoundManager.instance.placedCards) {
                var candidate = Instantiate(winnerAvatarPrefab, spawnContext);
                candidate.Initialize(placedCard.Key, placedCard.Value);
                candidates.Add(candidate);
            }
        }

        public void SpawnCards() {
            foreach (var candidate in candidates) {
                candidate.SpawnCard();
            }
        }

        public void RevealCards() {
            foreach (var candidate in candidates) {
                candidate.Reveal();
            }
        }

        public void ShowWinner() {
            foreach (var candidate in candidates.Where(candy => RoundManager.instance.CurrentWinners().ContainsKey(candy.owner))) {
                candidate.Win();
            }
        }

        public void CleanUp() {
            StopAllCoroutines();
            foreach (var candidate in candidates) {
                candidate.Die();
            }
            candidates.Clear();
        }
    }
}
