using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EQx.Game.Player {
    public class PlayerAvatar : MonoBehaviour {

        public CardPlayer observedPlayer = default;
        [SerializeField]
        Image shade = default;
        [SerializeField]
        TMP_Text nameText = default;

        [SerializeField]
        TMP_Text wins = default;



        [SerializeField]
        Color standardColor = default;
        [SerializeField]
        Color turnColor = default;
        [SerializeField]
        TMP_Text statusText = default;
        [SerializeField]
        Vector3 statusPath = default;
        [SerializeField]
        float statusDisplayDuration = 1;

        [SerializeField]
        string playedCardStatusMessage = "played card";

        int winsCounter;
        Vector3 statusOrigin = default;

        public void Initialize(CardPlayer player) {
            Debug.Log(name + "Initialize");
            observedPlayer = player;
            observedPlayer.onPlacedCard += CardPlacedListener;
            observedPlayer.onEndedTurn += EndedTurnListener;
            observedPlayer.onStartedTurn += StartedTurnListener;
            observedPlayer.onSetName += SetNameListener;
            observedPlayer.onWinRound += WinRoundListener;
            nameText.text = player.playerName;
            wins.text = "Wins: " + winsCounter;
            shade.color = standardColor;
            statusOrigin = statusText.transform.position;
            statusText.gameObject.SetActive(false);
        }

        private void WinRoundListener(CardPlayer player) {
            winsCounter++;
            wins.text = "Wins: " + winsCounter;
        }

        private void SetNameListener(CardPlayer player, string name) {
            nameText.text = name;
        }

        private void StartedTurnListener(CardPlayer player) {
            Debug.Log(name + "StartedTurnListener");
            shade.color = turnColor;
        }

        private void EndedTurnListener(CardPlayer player) {
            Debug.Log(name + "EndedTurnListener");
            shade.color = standardColor;
        }

        private void CardPlacedListener(CardPlayer player, int id) {
            StartCoroutine(ShowStatusText(playedCardStatusMessage));
        }

        IEnumerator ShowStatusText(string status) {
            statusText.gameObject.SetActive(true);
            statusText.transform.position = statusOrigin;
            statusText.text = status;
            float timer = 0;
            while (timer < statusDisplayDuration) {
                timer += Time.deltaTime;
                yield return null;
                statusText.transform.position = Vector3.Lerp(statusOrigin, statusOrigin + statusPath, timer / statusDisplayDuration);
            }
        }

        private void OnDisable() {
            if (observedPlayer) {
                observedPlayer.onPlacedCard -= CardPlacedListener;
                observedPlayer.onEndedTurn -= EndedTurnListener;
                observedPlayer.onStartedTurn -= StartedTurnListener;
            }
        }
    }
}
