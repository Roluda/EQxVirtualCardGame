using EQx.Game.CountryCards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using EQx.Game.Table;

namespace EQx.Game.Player {
    public class Hand : MonoBehaviour {
        [SerializeField]
        CountryCard cardPrefab;

        [SerializeField]
        float playOutDistance = 5;

        [SerializeField]
        int sortingOrderStart = 1;
        [SerializeField]
        Vector3 spaceBetweenCards = default;
        [SerializeField, Range(-90, 90)]
        float degreeBetweenCards = 20;
        [SerializeField]
        Transform fanAnchor = default;
        [SerializeField]
        Transform spawnLocation = default;
        [SerializeField]
        Transform despawnLocation = default;


        public List<CountryCard> cardInventory = new List<CountryCard>();
        public CountryCard placedCard = default;

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

        public void RemovePlacedCard() {
            if (placedCard != null) {
                Destroy(placedCard.gameObject);
            }
        }

        void PlacedCardListener(CardPlayer player, int id) {
            Debug.Log(name + "PlaceCardListener");
            var removedCard = cardInventory.Where(card => card.id == id).First();
            cardInventory.Remove(removedCard);
            removedCard.SetTargetPosition(despawnLocation.position);
            removedCard.SetTargetRotation(despawnLocation.rotation.eulerAngles);
            removedCard.PlayCard();
            removedCard.layer = sortingOrderStart - 2;
            placedCard = removedCard;
        }

        void ReceivedCardListener(CardPlayer player, int id) {
            Debug.Log(name + "ReceiveCardListener");
            var newCard = Instantiate(cardPrefab);
            newCard.data = CountryCardDatabase.instance.GetCountry(id);
            cardInventory.Add(newCard);
            newCard.onCardUnselected += CheckPlayDistance;
            newCard.transform.position = spawnLocation.transform.position;
            newCard.transform.rotation = spawnLocation.transform.rotation;
            newCard.DrawCard();
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
                localPlayer.PlaceCard(card.id);
            }
        }

        public void Initialize(CardPlayer player) {
            Debug.Log(name + "Initialize");
            localPlayer = player;
        }

        private void Awake() {
            CardPlayer.localPlayerReady += Initialize;
        }

        private void Start() {
            RoundManager.instance.onPlacingStarted += RemovePlacedCard;
        }

        // Update is called once per frame
        void Update() {
            var fan = cardInventory.Where(car => !car.selected).ToList();           ;
            foreach (var card in fan) {
                card.SetTargetPosition(CalculateFanPosition(fan.IndexOf(card), fan.Count));
                card.SetTargetRotation(CalculateFanRotation(fan.IndexOf(card), fan.Count));
                card.layer = sortingOrderStart + fan.IndexOf(card);
            }
        }

        Vector3 CalculateFanPosition(int index, int count) {
            Vector3 leftmost = fanAnchor.position - (spaceBetweenCards * (count - 1) / 2);
            return leftmost + spaceBetweenCards * index;
        }

        Vector3 CalculateFanRotation(int index, int count) {
            float leftmost = -degreeBetweenCards * (count - 1) / 2;
            return new Vector3(0, 0, leftmost + degreeBetweenCards * index);
        }

        private void OnDestroy() {
            CardPlayer.localPlayerReady -= Initialize;
        }
    }
}
