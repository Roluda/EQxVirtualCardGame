using EQx.Game.CountryCards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using EQx.Game.Table;
using UnityEngine.Assertions;

namespace EQx.Game.Player {
    public class Hand : MonoBehaviour {
        [SerializeField]
        DropZone dropZone = default;

        [SerializeField]
        CountryCard cardPrefab;

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
        [SerializeField]
        Transform placingLocation = default;
        [SerializeField]
        Transform bettingLocation = default;


        public List<CountryCard> cardInventory = new List<CountryCard>();
        public CountryCard placedCard = default;

        CardPlayer playerCache;
        public CardPlayer localPlayer {
            get => playerCache;
            set {
                if (playerCache && value != playerCache) {
                    playerCache.onStartedPlacing -= StartedPlacingListener;
                    playerCache.onEndedPlacing -= EndedPlacingListener;
                    playerCache.onStartedBetting -= StartedBettingListener;
                    playerCache.onEndedBetting -= EndedBettingListener;
                    playerCache.onPlacedCard -= PlacedCardListener;
                    playerCache.onReceivedCard -= ReceivedCardListener;
                }
                if (value && value != playerCache) {
                    value.onStartedPlacing += StartedPlacingListener;
                    value.onEndedPlacing += EndedPlacingListener;
                    value.onStartedBetting += StartedBettingListener;
                    value.onEndedBetting += EndedBettingListener;
                    value.onPlacedCard += PlacedCardListener;
                    value.onReceivedCard += ReceivedCardListener;
                }
                playerCache = value;
            }
        }

        void StartedPlacingListener(CardPlayer player, int round) {
            fanAnchor.position = placingLocation.position;
        }

        void EndedPlacingListener(CardPlayer player, int round) {

        }

        private void StartedBettingListener(CardPlayer player, int round) {
            fanAnchor.position = bettingLocation.position;
        }

        private void EndedBettingListener(CardPlayer player, int round) {
        }

        public void RemovePlacedCard() {
            if (placedCard != null) {
                Destroy(placedCard.gameObject);
                placedCard = null;
            }
        }

        void PlacedCardListener(CardPlayer player, int id) {
            Assert.IsTrue(!placedCard);
            var removedCard = cardInventory.Where(card => card.id == id).First();
            cardInventory.Remove(removedCard);
            removedCard.affordable = false;
            removedCard.SetTargetPosition(despawnLocation.position);
            removedCard.SetTargetRotation(despawnLocation.rotation.eulerAngles);
            removedCard.PlayCard();
            removedCard.order = sortingOrderStart - 3;
            placedCard = removedCard;
        }

        void ReceivedCardListener(CardPlayer player, int id) {
            var newCard = Instantiate(cardPrefab);
            newCard.data = CountryCardDatabase.instance.GetCountry(id);
            cardInventory.Add(newCard);
            newCard.onCardUnselected += CheckPlayDistance;
            newCard.onCardSelected += ShowDropZone;
            newCard.transform.position = spawnLocation.transform.position;
            newCard.transform.rotation = spawnLocation.transform.rotation;
            newCard.DrawCard();
        }

        void ShowDropZone(CountryCard card) {
            if (RoundManager.instance.GetParticipant(localPlayer).state == RoundState.Placing) {
                dropZone.Show();
            }
        }

        void CheckPlayDistance(CountryCard card) {
            if (dropZone.hovered) { 
                PlaceCard(card);
            }
            dropZone.Hide();
        }

        void PlaceCard(CountryCard card) {
            if (cardInventory.Contains(card)) {
                localPlayer.PlaceCard(card.id);
            }
        }

        public void Initialize(CardPlayer player) {
            CardPlayer.localPlayerReady -= Initialize;
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
                card.order = sortingOrderStart + fan.IndexOf(card);
                if(RoundManager.instance.GetParticipant(playerCache).state == RoundState.Placing) {
                    card.affordable = true;
                } else {
                    card.affordable = false;
                }
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
    }
}
