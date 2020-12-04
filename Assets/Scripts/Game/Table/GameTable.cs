using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using EQx.Game.CountryCards;
using EQx.Game.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

namespace EQx.Game.Table {
    public class GameTable : GameTableBehavior {

        public static GameTable instance = null;

        [SerializeField]
        float timeBetweenRounds = 20;
        [SerializeField]
        public CardDealer dealer = default;
        [SerializeField]
        List<EQxVariableType> possibleDemands = default;

        public UnityAction<EQxVariableType> onNewDemand;
        public UnityAction<CardPlayer> onPlayerSeated;
        public UnityAction<CardPlayer> onPlayerLeftTable;
        public UnityAction onTableUpdated;
        public UnityAction onRoundEnded;
        public UnityAction onRoundStarted;

        Dictionary<CardPlayer, int> placedCards = new Dictionary<CardPlayer, int>();
        public List<CardPlayer> registeredPlayers = new List<CardPlayer>();

        public EQxVariableType currentDemand;


        public void TakeSeat(CardPlayer player) {
            Debug.Log(name + "TakeSeat by" +player.name);
            player.onEndedTurn += EndTurnListener;
            player.onRequestedCard += dealer.RequestCard;
            player.onPlacedCard += PlaceCard;
            player.onPlacedCard += (cardPlayer, id) => player.CallEndTurn();
            registeredPlayers.Add(player);
            registeredPlayers.Sort((x, y) => x.networkObject.NetworkId.CompareTo(y.networkObject.NetworkId));
            onPlayerSeated?.Invoke(player);
            onTableUpdated?.Invoke();
            if(player.networkObject.IsServer) {
                NewRound();
            }
        }

        public void LeaveTable(CardPlayer player) {
            Debug.Log(name + "LeftTable by" + player.playerName);
            player.onEndedTurn -= EndTurnListener;
            player.onRequestedCard -= dealer.RequestCard;
            player.onPlacedCard -= PlaceCard;
            registeredPlayers.Remove(player);
            onPlayerLeftTable?.Invoke(player);
            onTableUpdated?.Invoke();
        }

        void EndTurnListener(CardPlayer player) {
            Debug.Log(name + "TurnEnded by" + player.playerName);
            int index = registeredPlayers.IndexOf(player);
            int maxIndex = registeredPlayers.Count - 1;
            if (index >= maxIndex) {
                EndRound();
            } else {
                index++;
                registeredPlayers[index].CallStartTurn();
            }
        }

        public override void SetDemand(RpcArgs args) {
            Debug.Log(name + "SetDemandRPC");
            currentDemand = (EQxVariableType)args.GetNext<int>();
            onNewDemand?.Invoke(currentDemand);
        }

        public void NewRound() {
            Debug.Log(name + "NewRound");
            foreach (var pair in placedCards) {
                dealer.CallDiscardCard(pair.Value);
                pair.Key.CallRequestCard();
            }
            placedCards.Clear();
            if (networkObject.IsOwner) {
                int newDemand = (int)possibleDemands[UnityEngine.Random.Range(0, possibleDemands.Count)];
                networkObject.SendRpc(RPC_SET_DEMAND, Receivers.AllBuffered, newDemand);
            }
            registeredPlayers[0].CallStartTurn();
            onRoundStarted?.Invoke();
        }

        void EndRound() {
            Debug.Log(name + "EndRound");
            onRoundEnded?.Invoke();
            foreach (var winner in FindWinners()) {
                winner.Key.CallWinRound();
            }
            StartCoroutine(WaitForNewRound());
        }


        public void PlaceCard(CardPlayer player, int id) {
            placedCards.Add(player, id);
        }


        // Start is called before the first frame update
        void Start() {
            NetworkManager.Instance.InstantiateCardPlayer();
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
            instance = null;
        }

        IEnumerator WaitForNewRound(){
            yield return new WaitForSeconds(timeBetweenRounds);
            NewRound();
        }


        public Dictionary<CardPlayer, int> FindWinners() {
            var winningCards = new Dictionary<CardPlayer, int>();
            foreach (var card in placedCards) {

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
            return winningCards;
        }
    }
}
