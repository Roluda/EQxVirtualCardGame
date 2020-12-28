using EQx.Game.CountryCards;
using EQx.Game.Table;
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
        SpriteRenderer shade = default;
        [SerializeField]
        TMP_Text nameText = default;
        [SerializeField]
        Color standardColor = default;
        [SerializeField]
        Color turnColor = default;

        [SerializeField]
        CountryCard countryCardPrefab;

        CountryCard placedCard;
        Seat mySeat;
        int winsCounter;

        public void Initialize(CardPlayer player) {
            Debug.Log(name + "Initialize");
            observedPlayer = player;
            observedPlayer.onPlacedCard += CardPlacedListener;
            observedPlayer.onEndedTurn += EndedTurnListener;
            observedPlayer.onStartedTurn += StartedTurnListener;
            observedPlayer.onSetName += SetNameListener;
            observedPlayer.onWinRound += WinRoundListener;
            nameText.text = player.playerName;
            shade.color = player.onTurn? turnColor : standardColor;
        }

        public void TakeSeat(Seat seat) {
            mySeat = seat;
            transform.position = seat.transform.position;
            transform.rotation = seat.transform.rotation;
        }

        public void RemovePlacedCard() {
            if (placedCard) {
                Destroy(placedCard.gameObject);
            }
        }

        private void WinRoundListener(CardPlayer player) {
            winsCounter++;
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
            if(mySeat && mySeat.cardPlace) {
                if (placedCard) {
                    Destroy(placedCard);
                }
                placedCard = Instantiate(countryCardPrefab, mySeat.cardPlace.position, mySeat.cardPlace.rotation);
                placedCard.SetTargetPosition(mySeat.cardPlace.position);
                placedCard.SetTargetRotation(mySeat.cardPlace.rotation.eulerAngles);
                placedCard.data = CountryCardDatabase.instance.GetCountry(id);
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
