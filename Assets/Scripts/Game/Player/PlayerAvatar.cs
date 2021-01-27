using EQx.Game.CountryCards;
using EQx.Game.Investing;
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
        TMP_Text cashText = default;
        [SerializeField]
        string cashTextPrefix = "EliteCoins: ";
        [SerializeField]
        float cashGainInterval = 1.3f;
        [SerializeField]
        Color standardColor = default;
        [SerializeField]
        Color turnColor = default;

        [SerializeField]
        CountryCard countryCardPrefab;
        [SerializeField]
        CommitmentPile commitmentPilePrefab = default;

        CountryCard placedCard;
        CommitmentPile playerPile;
        Seat mySeat;


        int currentCash = 0;
        int targetCash = 0;
        float timer = 0;

        private void Update() {
            timer += Time.deltaTime;
            if(timer >=cashGainInterval) {
                timer = 0;
                if (currentCash < targetCash) {
                    currentCash++;
                    cashText.text = cashTextPrefix + currentCash;
                }else if(currentCash> targetCash) {
                    currentCash--;
                    cashText.text = cashTextPrefix + currentCash;
                }
            }
        }

        public void Initialize(CardPlayer player) {
            Debug.Log(name + "Initialize");
            observedPlayer = player;
            observedPlayer.onPlacedCard += CardPlacedListener;
            observedPlayer.onEndedTurn += EndedTurnListener;
            observedPlayer.onStartedTurn += StartedTurnListener;
            observedPlayer.onSetName += SetNameListener;
            InvestmentManager.instance.onCapitalUpdated += CapitalUpdatedListener;
            nameText.text = player.playerName;
            shade.color = player.onTurn? turnColor : standardColor;
            playerPile = Instantiate(commitmentPilePrefab, transform);
            playerPile.Initialize(player);
        }

        private void CapitalUpdatedListener(CardPlayer player) {
            if(player == observedPlayer) {
                targetCash = InvestmentManager.instance.Capital(player);
            }
        }

        public void TakeSeat(Seat seat) {
            mySeat = seat;
            transform.position = seat.transform.position;
            transform.rotation = seat.transform.rotation;
            playerPile.transform.position = seat.commitmentPlace.position;
        }

        public void RemovePlacedCard() {
            if (placedCard) {
                Destroy(placedCard.gameObject);
            }
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
