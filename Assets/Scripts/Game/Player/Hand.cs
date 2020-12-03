using EQx.Game.CountryCards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace EQx.Game.Player {
    public class Hand : MonoBehaviour {
        [SerializeField]
        int initialCards = 3;

        [SerializeField]
        CountryCard cardPrefab;

        [SerializeField]
        float playOutDistance = 5;

        [SerializeField]
        Vector3 spaceBetweenCards = default;
        [SerializeField]
        Vector3 fanWeightDirection = default;
        [SerializeField]
        Transform fanAnchor = default;
        [SerializeField]
        Transform spawnLocation = default;
        [SerializeField]
        Transform despawnLocation = default;
        [SerializeField]
        Transform lookAtTransform = default;


        public List<CountryCard> cardInventory = new List<CountryCard>();

        CardPlayer playerCache;
        public CardPlayer localPlayer {
            get => playerCache;
            set {
                if (playerCache && value != playerCache) {
                    playerCache.onEndedTurn -= EndedTurnListener;
                    playerCache.onPlacedCard -= PlacedCardListener;
                    playerCache.onReceivedCard -= ReceivedCardListener;
                    playerCache.onStartedTurn -= StartedTurnListener;
                }
                if (value && value != playerCache) {
                    value.onEndedTurn += EndedTurnListener;
                    value.onPlacedCard += PlacedCardListener;
                    value.onReceivedCard += ReceivedCardListener;
                    value.onStartedTurn += StartedTurnListener;
                }
                playerCache = value;
            }
        }

        void EndedTurnListener(CardPlayer player) {

        }

        void StartedTurnListener(CardPlayer player) {

        }

        void PlacedCardListener(CardPlayer player, int id) {
            Debug.Log(name + "PlaceCardListener");
            var removedCard = cardInventory.Where(card => card.data.cardID == id).First();
            cardInventory.Remove(removedCard);
            removedCard.motor.targetPosition = despawnLocation.position;
        }

        void ReceivedCardListener(CardPlayer player, int id) {
            Debug.Log(name + "ReceiveCardListener");
            var newCard = Instantiate(cardPrefab);
            newCard.data = CountryCardDatabase.instance.GetCountry(id);
            cardInventory.Add(newCard);
            newCard.onCardUnselected += CheckPlayDistance;
            newCard.transform.position = spawnLocation.transform.position;
        }

        void CheckPlayDistance(CountryCard card) {
            Debug.Log(name + "CheckPlayDistance");
            if ((card.transform.position - fanAnchor.transform.position).magnitude > playOutDistance) { 
                PlaceCard(card);
            }
        }

        void PlaceCard(CountryCard card) {
            Debug.Log(name + "PlaceCard");
            if (cardInventory.Contains(card)) {
                localPlayer.CallPlaceCard(card.data.cardID);
            }
        }

        public void Initialize(CardPlayer player) {
            Debug.Log(name + "Initialize");
            localPlayer = player;
            for (int i = 0; i < initialCards; i++) {
                localPlayer.CallRequestCard();
            }
        }

        // Update is called once per frame
        void Update() {
            var fan = cardInventory.Where(car => !car.selected).ToList();           ;
            foreach (var card in fan) {
                card.motor.targetPosition = CalculateFanPosition(fan.IndexOf(card));
                card.motor.targetUpDirection = card.transform.position - fanAnchor.position + fanWeightDirection;
                card.motor.targetLookDirection = lookAtTransform.position - card.transform.position;
            }
        }

        Vector3 CalculateFanPosition(int index) {
            return fanAnchor.position + spaceBetweenCards * --index / 2;
        }
    }
}
