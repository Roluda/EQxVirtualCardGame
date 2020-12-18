using EQx.Game.CountryCards;
using EQx.Game.Player;
using Photon.Pun;
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
        public UnityAction onRoundStarted;

        public Dictionary<CardPlayer, int> placedCards = new Dictionary<CardPlayer, int>();
        public List<CardPlayer> registeredPlayers = new List<CardPlayer>();

        public EQxVariableType currentDemand;


        public void Register(CardPlayer player) {
            Debug.Log(name + ".Register by" + player.name);
            player.onEndedTurn += EndTurnListener;
            player.onPlacedCard += PlaceCard;
            player.onPlacedCard += (cardPlayer, id) => player.EndTurn();
            registeredPlayers.Add(player);
            registeredPlayers.Sort((x, y) => x.photonView.Owner.ActorNumber.CompareTo(y.photonView.Owner.ActorNumber));
            for(int i=0; i < registeredPlayers.Count; i++) {
                registeredPlayers[i].seatNumber = i;
            }
            onPlayerRegister?.Invoke(player);
            onRegisterUpdate?.Invoke();
            if (PhotonNetwork.IsMasterClient) {
                if (registeredPlayers.Count == 1) {
                    StartPlacingRound();
                }
            }
        }

        public void Unregister(CardPlayer player) {
            Debug.Log(name + ".Unregister by" + player.playerName);
            player.onEndedTurn -= EndTurnListener;
            player.onPlacedCard -= PlaceCard;
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
                EndPlacingRound();
            } else {
                index++;
                registeredPlayers[index].StartTurn();
            }
        }

        [PunRPC]
        void SetDemandRPC(int demand) {
            Debug.Log(name + ".SetDemandRPC");
            currentDemand = (EQxVariableType)demand;
            onNewDemand?.Invoke(currentDemand);
        }

        public void SetRandomDemand() {
            if (PhotonNetwork.IsMasterClient) {
                int newDemand = (int)possibleDemands[Random.Range(0, possibleDemands.Count)];
                photonView.RPC("SetDemandRPC", RpcTarget.AllBuffered, newDemand);
            }
        }

        [PunRPC]
        void StartPlacingRoundRPC() {
            Debug.Log(name + ".StartPlacingRoundRPC");
            foreach (var pair in placedCards) {
                CardDealer.instance.DiscardCard(pair.Value);
                pair.Key.RequestCard();
            }
            SetRandomDemand();
            placedCards.Clear();
            registeredPlayers[0].StartTurn();
            onRoundStarted?.Invoke();
        }

        public void StartPlacingRound() {
            Debug.Log(name + ".StartPlacingRound");
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC("StartPlacingRoundRPC", RpcTarget.AllBuffered);
            }
        }

        [PunRPC]
        void EndPlacingRoundRPC() {
            Debug.Log(name + ".EndPlacingRoundRPC");
            onPlacingEnded?.Invoke();
            foreach (var winner in CurrentWinners()) {
                winner.Key.CallWinRound();
            }
        }

        public void EndPlacingRound() {
            Debug.Log(name + ".EndPlacingRound");
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC("EndPlacingRoundRPC", RpcTarget.AllBuffered);
            }
        }

        public void PlaceCard(CardPlayer player, int id) {
            placedCards.Add(player, id);
        }

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

        public Dictionary<CardPlayer, int> CurrentWinners() {
            var winningCards = new Dictionary<CardPlayer, int>();
            foreach (var card in placedCards) {
                if (registeredPlayers.Contains(card.Key)) {
                    if (winningCards.Count == 0) {
                        winningCards.Add(card.Key, card.Value);
                    } else {
                        float candidateValue = CountryCardDatabase.instance.GetCountry(card.Value).GetValue(currentDemand);
                        float beatValue = CountryCardDatabase.instance.GetCountry(winningCards.First().Value).GetValue(currentDemand);
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

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        }
    }
}
