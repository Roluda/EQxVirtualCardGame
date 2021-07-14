using EQx.Game.CountryCards;
using EQx.Game.Player;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EQx.Game.Table {
    public class CardDealer : MonoBehaviourPunCallbacks, IPunObservable {

        public static CardDealer instance = null;

        [SerializeField]
        int initialCards = 3;

        List<int> drawPile;
        List<int> discardPile;

        List<CardInventory> playerInventories = new List<CardInventory>();

        public void Register(CardPlayer player) {
            player.onRequestedCard += RequestCard;
            player.onPlacedCard += PlacedCard;
            player.onReceivedCard += ReceivedCard;

            string userID = player.photonView.Owner.UserId;
            var inventory = playerInventories.FirstOrDefault(inv => inv.ownerID == userID);
            if (inventory == null) {
                inventory = new CardInventory() {
                    owner = player,
                    ownerID = userID,
                };
                playerInventories.Add(inventory);
                if (PhotonNetwork.IsMasterClient) {
                    for (int i = 0; i < initialCards; i++) {
                        DealCard(player);
                    }
                }
            } else {
                inventory.owner = player;
                int[] cards = inventory.cards.ToArray();
                inventory.cards.Clear();
                foreach(int card in cards) {
                    player.ReceiveCard(card);
                }
            }
        }

        public void Unregister(CardPlayer player) {
            player.onRequestedCard -= RequestCard;
            player.onPlacedCard -= PlacedCard;
            player.onReceivedCard -= ReceivedCard;
        }

        [PunRPC]
        void DiscardCardRPC(int id) {
            Logger.Log($"{name}.{nameof(DiscardCardRPC)}({id})");
            discardPile.Add(id);
        }

        public void DiscardCard(int id) {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC("DiscardCardRPC", RpcTarget.AllBuffered, id);
            }
        }

        [PunRPC]
        void ReshuffleRPC() {
            Logger.Log($"{name}.{nameof(ReshuffleRPC)}");
            drawPile.AddRange(discardPile);
            discardPile.Clear();
        }

        [PunRPC]
        void DrawCardRPC(int id) {
            Logger.Log($"{name}.{nameof(DrawCardRPC)}({id})");
            drawPile.Remove(id);
        }

        public void RequestCard(CardPlayer player) {
            DealCard(player);
        }

        void DealCard(CardPlayer player) {
            if (PhotonNetwork.IsMasterClient) {
                if (drawPile.Count == 0) {
                    photonView.RPC("ReshuffleRPC", RpcTarget.All);
                }
                if (drawPile.Count > 0) {
                    float preferredPriority = UnityEngine.Random.Range(1, 10);
                    var matchingCards = drawPile.Where(card => GetCardPriority(card) > preferredPriority).ToList();
                    if (matchingCards.Count > 0) {
                        int randomIndex = UnityEngine.Random.Range(0, matchingCards.Count - 1);
                        int randomCard = drawPile[randomIndex];
                        photonView.RPC("DrawCardRPC", RpcTarget.AllBuffered, randomCard);
                        player.ReceiveCard(randomCard);
                    } else {
                        int randomIndex = UnityEngine.Random.Range(0, drawPile.Count - 1);
                        int randomCard = drawPile[randomIndex];
                        photonView.RPC("DrawCardRPC", RpcTarget.AllBuffered, randomCard);
                        player.ReceiveCard(randomCard);
                    }
                }
            }
        }

        private void ReceivedCard(CardPlayer player, int card) {
            GetInventory(player).cards.Add(card);
        }

        private void PlacedCard(CardPlayer player, int card) {
            GetInventory(player).cards.Remove(card);
        }

        private void Awake() {
            if (instance != null) {
                Destroy(gameObject);
            } else {
                instance = this;
            }
        }

        // Start is called before the first frame update
        void Start() {
            BuildDeck();
        }

        float GetCardPriority(int card) {
            return CountryCardDatabase.instance.data.eqxCountryData[card].cardPriority;
        }

        void BuildDeck() {
            drawPile = new List<int>();
            discardPile = new List<int>();
            for (int i = 0; i < CountryCardDatabase.instance.length; i++) {
                drawPile.Add(i);
            }
        }

        CardInventory GetInventory(CardPlayer player) {
            return playerInventories.First(inventory => inventory.owner == player);
        }

        private void OnDestroy() {
            if (instance == this)
                instance = null;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        }
    }
}
