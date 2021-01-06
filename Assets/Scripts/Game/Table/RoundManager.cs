using EQx.Game.CountryCards;
using EQx.Game.Investing;
using EQx.Game.Player;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

namespace EQx.Game.Table {
    public class RoundManager : MonoBehaviourPunCallbacks , IPunObservable {

        public static RoundManager instance = null;
        [SerializeField]
        List<EQxVariableType> possibleDemands = default;

        public UnityAction<EQxVariableType> onNewDemand;
        public UnityAction<CardPlayer> onPlayerRegister;
        public UnityAction<CardPlayer> onPlayerUnregister;
        public UnityAction onRegisterUpdate;
        public UnityAction onPlacingEnded;
        public UnityAction onPlacingStarted;
        public UnityAction onBettingEnded;
        public UnityAction onBettingStarted;

        public Dictionary<CardPlayer, int> placedCards = new Dictionary<CardPlayer, int>();
        public List<CardPlayer> registeredPlayers = new List<CardPlayer>();

        public EQxVariableType currentDemand;
        public bool bettingRound = false;



        public void Register(CardPlayer player) {
            Debug.Log(name + ".Register by" + player.name);
            player.onEndedTurn += EndTurnListener;
            player.onPlacedCard += PlacedCardListener;
            registeredPlayers.Add(player);
            registeredPlayers.Sort((x, y) => x.photonView.Owner.ActorNumber.CompareTo(y.photonView.Owner.ActorNumber));
            for(int i=0; i < registeredPlayers.Count; i++) {
                registeredPlayers[i].seatNumber = i;
            }
            onPlayerRegister?.Invoke(player);
            onRegisterUpdate?.Invoke();
            if (PhotonNetwork.IsMasterClient) {
                if (registeredPlayers.Count == 1) {
                    StartRound();
                }
            }
        }

        public void Unregister(CardPlayer player) {
            Debug.Log(name + ".Unregister by" + player.playerName);
            player.onEndedTurn -= EndTurnListener;
            player.onPlacedCard -= PlacedCardListener;
            if (player.onTurn) {
                EndTurnListener(player);
            }
            registeredPlayers.Remove(player);
            onPlayerUnregister?.Invoke(player);
            onRegisterUpdate?.Invoke();
        }

        void EndTurnListener(CardPlayer player) {
            Debug.Log(name + ".TurnEnded by" + player.playerName);
            int index = registeredPlayers.IndexOf(player);
            int maxIndex = registeredPlayers.Count - 1;
            if (index >= maxIndex) {
                if (bettingRound) {
                    EndBettingRound();
                } else {
                    EndPlacingRound();
                }
            } else {
                index++;
                registeredPlayers[index].StartTurn();
            }
        }


        public void PlacedCardListener(CardPlayer player, int id) {
            placedCards.Add(player, id);
            player.EndTurn();
        }


        #region RoundBehaviour


        public void StartRound() {
            SetDemand();
            RequestBlinds();
        }

        void SetDemand() {
            Debug.Log(name + ".SetDemand");
            if (PhotonNetwork.IsMasterClient) {
                int newDemand = (int)possibleDemands[UnityEngine.Random.Range(0, possibleDemands.Count)];
                photonView.RPC("SetDemandRPC", RpcTarget.AllBuffered, newDemand);
            }
        }

        [PunRPC]
        void SetDemandRPC(int demand) {
            Debug.Log(name + ".SetDemandRPC");
            currentDemand = (EQxVariableType)demand;
            onNewDemand?.Invoke(currentDemand);
        }

        void RequestBlinds() {
            Debug.Log(name + ".RequestBlinds");
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC("RequestBlindsRPC", RpcTarget.AllBuffered);
            }
        }

        [PunRPC]
        void RequestBlindsRPC() {
            Debug.Log(name + ".RequestBlindsRPC");
            foreach (var player in registeredPlayers) {
                player.PayBlind();
            }
        }

        private void BlindsPayedListener() {
            Debug.Log(name + "BlindsPayedListener");
            StartPlacingRound();
        }

        void StartPlacingRound() {
            Debug.Log(name + ".StartPlacingRound");
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC("StartPlacingRoundRPC", RpcTarget.AllBuffered);
            }
        }

        [PunRPC]
        void StartPlacingRoundRPC() {
            Debug.Log(name + ".StartPlacingRoundRPC");
            foreach (var pair in placedCards) {
                CardDealer.instance.DiscardCard(pair.Value);
                if (InvestmentManager.instance.SufficientBlind(pair.Key)) {
                    pair.Key.RequestCard();
                }
            }
            foreach (var player in registeredPlayers) {
                player.cardPlaced = false;
            }
            bettingRound = false;
            placedCards.Clear();
            registeredPlayers[0].StartTurn();
            onPlacingStarted?.Invoke();
        }

        void EndPlacingRound() {
            Debug.Log(name + ".EndPlacingRound");
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC("EndPlacingRoundRPC", RpcTarget.AllBuffered);
            }
        }

        [PunRPC]
        void EndPlacingRoundRPC() {
            Debug.Log(name + ".EndPlacingRoundRPC");
            onPlacingEnded?.Invoke();
            StartBettingRound();
        }

        void StartBettingRound() {
            Debug.Log(name + ".StartBettingRound");
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC("StartBettingRoundRPC", RpcTarget.AllBuffered);
            }
        }

        [PunRPC]
        void StartBettingRoundRPC() {
            Debug.Log(name + ".StartBettingRoundRPC");
            bettingRound = true;
            registeredPlayers[0].StartTurn();
            onBettingStarted?.Invoke();
        }

        void EndBettingRound() {
            Debug.Log(name + ".EndBettingRound");
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC("EndBettingRoundRPC", RpcTarget.AllBuffered);
            }
        }

        [PunRPC]
        void EndBettingRoundRPC() {
            Debug.Log(name + ".EndBettingRoundRPC");
            onBettingEnded?.Invoke();
            List<CardPlayer> winners = CurrentWinners().Keys.ToList();
            foreach (var winner in winners) {
                winner.WinRound();
            }
            InvestmentManager.instance.WinPrize(winners);            
        }



        #endregion

        #region Setup

        private void Awake() {
            if (instance) {
                Destroy(gameObject);
                return;
            } else {
                instance = this;
            }
        }

        void Start() {
            InvestmentManager.instance.onAllBlindsPayed += BlindsPayedListener;
        }

        private void OnDestroy() {
            if (instance == this)
                instance = null;
        }

        #endregion

        #region Utility

        public Dictionary<CardPlayer, int> CurrentWinners() {
            var winningCards = new Dictionary<CardPlayer, int>();
            foreach (var card in placedCards) {
                if (registeredPlayers.Contains(card.Key)) {
                    if (winningCards.Count == 0) {
                        winningCards.Add(card.Key, card.Value);
                    } else {
                        float candidateValue = CalculateTotalValue(card.Key, card.Value);
                        float beatValue = CalculateTotalValue(winningCards.First().Key, winningCards.First().Value);
                        if (candidateValue > beatValue) {
                            winningCards.Clear();
                            winningCards.Add(card.Key, card.Value);
                        }
                        if (candidateValue == beatValue) {
                            winningCards.Add(card.Key, card.Value);
                        }
                    }
                }
            }
            return winningCards;
        }

        #endregion

        float CalculateTotalValue(CardPlayer player, int id) {
            float baseValue = CountryCardDatabase.instance.GetCountry(id).GetValue(currentDemand);
            float bonusValue = InvestmentManager.instance.BonusValue(player);
            return baseValue + bonusValue;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        }
    }
}
