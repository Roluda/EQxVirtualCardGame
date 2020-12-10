using EQx.Game.CountryCards;
using EQx.Game.Player;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

namespace EQx.Game.Table {
    public class RoundManager : MonoBehaviourPunCallbacks {

        public static RoundManager instance = null;

        [SerializeField]
        float timeBetweenRounds = 20;
        [SerializeField]
        int initialCards = 3;
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


        public void Register(CardPlayer player) {
            Debug.Log(name + "TakeSeat by" + player.name);
            player.onEndedTurn += EndTurnListener;
            player.onRequestedCard += CardDealer.instance.RequestCard;
            player.onPlacedCard += PlaceCard;
            player.onPlacedCard += (cardPlayer, id) => player.EndTurn();
            registeredPlayers.Add(player);
            registeredPlayers.Sort((x, y) => x.photonView.Owner.ActorNumber.CompareTo(y.photonView.Owner.ActorNumber));
            onPlayerSeated?.Invoke(player);
            onTableUpdated?.Invoke();
            for (int i = 0; i< initialCards; i++) {
                player.RequestCard();
            }
            if (PhotonNetwork.IsMasterClient) {
                NewRound();
            }
        }

        public void Unregister(CardPlayer player) {
            Debug.Log(name + "LeftTable by" + player.playerName);
            player.onEndedTurn -= EndTurnListener;
            player.onRequestedCard -= CardDealer.instance.RequestCard;
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
                registeredPlayers[index].StartTurn();
            }
        }

        [PunRPC]
        public void SetDemand(int demand) {
            Debug.Log(name + "SetDemandRPC");
            currentDemand = (EQxVariableType)demand;
            onNewDemand?.Invoke(currentDemand);
        }

        public void NewRound() {
            Debug.Log(name + "NewRound");
            placedCards.Clear();
            if (PhotonNetwork.IsMasterClient) {
                foreach (var pair in placedCards) {
                    CardDealer.instance.DiscardCard(pair.Value);
                    pair.Key.RequestCard();
                }
                int newDemand = (int)possibleDemands[UnityEngine.Random.Range(0, possibleDemands.Count)];
                photonView.RPC("SetDemand", RpcTarget.AllBuffered, newDemand);
            }
            registeredPlayers[0].StartTurn();
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

        IEnumerator WaitForNewRound() {
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
