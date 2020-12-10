using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EQx.Game.Player {
    public class EndTurnButton : MonoBehaviour {
        [SerializeField]
        Button button;

        private void Awake() {
            CardPlayer.localPlayerReady += AddListeners;
        }

        private void AddListeners(CardPlayer player) {
            player.onEndedTurn += EndedTurnListener;
            player.onStartedTurn += StartedTurnListener;
            button.gameObject.SetActive(false);
            button.onClick.AddListener(delegate { player.EndTurn(); });
            button.onClick.AddListener(delegate { button.gameObject.SetActive(false); });
        }

        private void StartedTurnListener(CardPlayer arg0) {
            button.gameObject.SetActive(true);
        }

        private void EndedTurnListener(CardPlayer arg0) {
            button.gameObject.SetActive(false);
        }
    }
}
