using EQx.Game.CountryCards;
using EQx.Game.Investing;
using EQx.Game.Player;
using EQx.Menu;
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
        RoundData defaultRound = default;
        [SerializeField]
        RoundData[] scriptedRounds = default;
        [SerializeField]
        public int maxRounds;

        public UnityAction<EQxVariableType> onNewDemand;
        public UnityAction<CardPlayer> onPlayerRegister;
        public UnityAction<CardPlayer> onPlayerUnregister;
        public UnityAction onRegisterUpdate;
        public UnityAction onPlacingEnded;
        public UnityAction onPlacingStarted;
        public UnityAction onBettingEnded;
        public UnityAction onBettingStarted;
        public UnityAction onNewRound;
        public UnityAction onGameEnd;

        public List<CardPlayer> registeredPlayers = new List<CardPlayer>();
        public CardPlayer winner = default;

        public EQxVariableType currentDemand;

        public int currentRound = 0;
        bool inRound = false;
        bool gameOver = false;
        public bool extractionRound = false;

        public void Register(CardPlayer player) {
            Debug.Log(name + ".Register by" + player.name);
            player.onPayedBlind += PayedBlindListener;
            player.onPlacedCard += PlacedCardListener;
            player.onEndedPlacing += EndPlacingListener;
            player.onEndedBetting += EndBettingListener;

            registeredPlayers.Add(player);
            registeredPlayers.Sort((x, y) => x.photonView.Owner.ActorNumber.CompareTo(y.photonView.Owner.ActorNumber));
            for (int i = 0; i < registeredPlayers.Count; i++) {
                registeredPlayers[i].seatNumber = i;
            }
            onPlayerRegister?.Invoke(player);
            onRegisterUpdate?.Invoke();
            if (inRound) {
                player.PayBlind();
                player.StartPlacing();
            }
            if (PhotonNetwork.IsMasterClient) {
                if (registeredPlayers.Count == 1) {
                    NewRound();
                }
            }
            UpdateRoomProperties();
        }

        public void Unregister(CardPlayer player) {
            Debug.Log(name + ".Unregister by" + player.playerName);
            player.onEndedPlacing -= EndPlacingListener;
            player.onEndedBetting -= EndBettingListener;
            player.onPlacedCard -= PlacedCardListener;
            player.onPayedBlind -= PayedBlindListener;
            player.seatNumber = -1;
            registeredPlayers.Remove(player);
            onPlayerUnregister?.Invoke(player);
            onRegisterUpdate?.Invoke();
            TryEndRound();
            UpdateRoomProperties();
        }

        void PayedBlindListener(CardPlayer player) {
            player.RequestCard();
        }

        void PlacedCardListener(CardPlayer player, int id) {
            player.baseValue = CountryCardDatabase.instance.GetCountry(id).GetValue(currentDemand);
            player.EndPlacing();
        }

        void EndPlacingListener(CardPlayer player) {
            player.StartBetting();
        }

        void EndBettingListener(CardPlayer player) {
            TryEndRound();
        }

        #region RoundBehaviour
        public void NewRound() {
            Debug.Log(name + ".NewRound");
            if (PhotonNetwork.IsMasterClient && !inRound) {
                if (currentRound >= maxRounds) {
                    photonView.RPC("EndGameRPC", RpcTarget.AllBuffered);
                } else {
                    photonView.RPC("NewRoundRPC", RpcTarget.AllBuffered);
                }
            }
        }

        [PunRPC]
        void NewRoundRPC() {
            Debug.Log(name + ".NewRoundRPC");
            currentRound++;
            inRound = true;
            onNewRound?.Invoke();
            SetDemand();
            StartPlacingRound();
            UpdateRoomProperties();
        }

        void SetDemand() {
            Debug.Log(name + ".SetDemand");
            if (PhotonNetwork.IsMasterClient) {
                var newDemand = currentRound <= scriptedRounds.Length
                    ? scriptedRounds[currentRound - 1].randomDemand
                    : defaultRound.randomDemand;
                photonView.RPC("SetDemandRPC", RpcTarget.AllBuffered, (int)newDemand);
            }
        }

        [PunRPC]
        void SetDemandRPC(int demand) {
            Debug.Log(name + ".SetDemandRPC");
            currentDemand = (EQxVariableType)demand;
            onNewDemand?.Invoke(currentDemand);
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
            foreach (var player in registeredPlayers) {
                if (!(player.state == PlayerState.Unregistered)) {
                    CardDealer.instance.DiscardCard(player.placedCardID);
                }
                player.StartPlacing();
                player.PayBlind();
            }
            onPlacingStarted?.Invoke();
        }

        void TryEndRound() {
            Debug.Log(name + ".TryEndRound");
            if (registeredPlayers.All(cardPlayer => cardPlayer.state == PlayerState.Betted || cardPlayer.state == PlayerState.Unregistered)) {
                if (PhotonNetwork.IsMasterClient) {
                    photonView.RPC("EndBettingRoundRPC", RpcTarget.AllBuffered);
                }
            }
        }

        [PunRPC]
        void EndBettingRoundRPC() {
            Debug.Log(name + ".EndBettingRoundRPC");
            inRound = false;
            extractionRound = InvestmentManager.instance.prize < 0;
            winner = extractionRound
                ? registeredPlayers.Aggregate((x, y) => x.combinedValue < y.combinedValue ? x : y)
                : registeredPlayers.Aggregate((x, y) => x.combinedValue > y.combinedValue ? x : y);
            foreach(var player in registeredPlayers) {
                if(player == winner) {
                    player.Win();
                } else {
                    player.Lose();
                }
            }
            onBettingEnded?.Invoke();
        }

        void EndGame() {
            Debug.Log(name + ".EndGame");
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC("EndGameRPC", RpcTarget.AllBuffered);
            }
        }

        [PunRPC]
        void EndGameRPC() {
            Debug.Log(name + ".EndGameRPC");
            if (!gameOver) {
                gameOver = true;
                currentRound++;
                onGameEnd?.Invoke();
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
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

        void Start() {
            if (PhotonNetwork.IsConnected) {
                maxRounds = (int)PhotonNetwork.CurrentRoom.CustomProperties[TableBrowser.MAX_ROUNDS];
            }
        }

        #endregion
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        }


        private void UpdateRoomProperties() {
            if (PhotonNetwork.IsMasterClient) {
                var props = PhotonNetwork.CurrentRoom.CustomProperties;
                props[TableBrowser.CONNECTED_PLAYERS] = registeredPlayers.Select(player => player.playerName).ToArray();
                props[TableBrowser.CURRENT_ROUND] = currentRound;
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }
        }
    }
}
