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
        public enum RoundState {
            pre,
            placing,
            betting,
            post
        }

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
        public List<PlayerStats> playerStats = new List<PlayerStats>();
        public List<CardPlayer> registeredPlayers = new List<CardPlayer>();
        public List<CardPlayer> preRegisteredPlayers = new List<CardPlayer>();
        public List<CardPlayer> winners = new List<CardPlayer>();

        public EQxVariableType currentDemand;
        public RoundState roundState = RoundState.pre;

        public void Register(CardPlayer player) {
            Debug.Log(name + ".Register by" + player.name);
            if (OpenForRegistration()) {
                player.onEndedTurn += EndTurnListener;
                player.onPlacedCard += PlacedCardListener;
                player.onPayedBlind += PayedBlindListener;
                registeredPlayers.Add(player);
                registeredPlayers.Sort((x, y) => x.photonView.Owner.ActorNumber.CompareTo(y.photonView.Owner.ActorNumber));
                for (int i = 0; i < registeredPlayers.Count; i++) {
                    registeredPlayers[i].seatNumber = i;
                }
                onPlayerRegister?.Invoke(player);
                onRegisterUpdate?.Invoke();
                if (roundState == RoundState.placing) {
                    player.cardPlaced = false;
                    player.PayBlind();
                    player.StartTurn();
                }
                if (PhotonNetwork.IsMasterClient) {
                    if (registeredPlayers.Count == 1) {
                        NewRound();
                    }
                }
            } else {
                preRegisteredPlayers.Add(player);
                player.seatNumber = registeredPlayers.Count;
                onRegisterUpdate?.Invoke();
            }
        }

        public void Unregister(CardPlayer player) {
            Debug.Log(name + ".Unregister by" + player.playerName);
            player.onEndedTurn -= EndTurnListener;
            player.onPlacedCard -= PlacedCardListener;
            player.onPayedBlind -= PayedBlindListener;
            if (player.onTurn) {
                EndTurnListener(player);
            }
            player.seatNumber = -1;
            registeredPlayers.Remove(player);
            onPlayerUnregister?.Invoke(player);
            onRegisterUpdate?.Invoke();
        }

        private void PayedBlindListener(CardPlayer player) {
            player.RequestCard();
        }

        public void PlacedCardListener(CardPlayer player, int id) {
            placedCards.Add(player, id);
            player.EndTurn();
        }

        void EndTurnListener(CardPlayer player) {
            Debug.Log(name + ".TurnEnded by" + player.playerName);
            int index = registeredPlayers.IndexOf(player);
            int maxIndex = registeredPlayers.Count - 1;
            switch (roundState) {
                case RoundState.placing:
                    if(registeredPlayers.All(cardPlayer => cardPlayer.cardPlaced)) {
                        EndPlacingRound();
                    }
                    if (index >= maxIndex) {
                        //EndPlacingRound();
                    } else {
                        index++;
                        //registeredPlayers[index].PayBlind();
                        //registeredPlayers[index].StartTurn();
                    }
                    break;
                case RoundState.betting:
                    if(registeredPlayers.All(cardPlayer => cardPlayer.invested)) {
                        EndBettingRound();
                    }
                    if (index >= maxIndex) {
                        //EndBettingRound();
                    } else {
                        index++;
                        //registeredPlayers[index].StartTurn();
                    }
                    break;
                default:
                    break;
            }
        }



        #region RoundBehaviour
        public void NewRound() {
            Debug.Log(name + ".NewRound");
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC("NewRoundRPC", RpcTarget.AllBuffered);
            }
        }

        [PunRPC]
        void NewRoundRPC() {
            Debug.Log(name + ".NewRoundRPC");
            roundState = RoundState.pre;
            foreach (var player in preRegisteredPlayers) {
                Register(player);
            }
            preRegisteredPlayers.Clear();
            SetDemand();
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
            roundState = RoundState.placing;
            foreach (var pair in placedCards) {
                CardDealer.instance.DiscardCard(pair.Value);
            }
            foreach (var player in registeredPlayers) {
                player.cardPlaced = false;
                player.invested = false;
                player.StartTurn();
                player.PayBlind();
            }
            placedCards.Clear();
            playerStats.Clear();
            //registeredPlayers[0].StartTurn();
            //registeredPlayers[0].PayBlind();
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
            roundState = RoundState.betting;
            registeredPlayers.ForEach(player => player.StartTurn());
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
            winners = CurrentWinners().Keys.ToList();
            foreach (var player in registeredPlayers) {
                var stats = new PlayerStats();
                stats.player = player;
                stats.placedCard = placedCards[player];
                stats.baseValue = CountryCardDatabase.instance.GetCountry(stats.placedCard).GetValue(currentDemand);
                stats.bonusValue = InvestmentManager.instance.BonusValue(player);
                stats.won = winners.Contains(player);
                playerStats.Add(stats);
            }
            roundState = RoundState.post;
            onBettingEnded?.Invoke();
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

        public bool OpenForRegistration() {
            return roundState != RoundState.betting;
        }
    }
}
