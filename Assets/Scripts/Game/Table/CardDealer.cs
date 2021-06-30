using EQx.Game.CountryCards;
using EQx.Game.Player;
using Photon.Pun;
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

        [PunRPC]
        void DiscardCardRPC(int id) {
            Debug.Log(name + ".DiscardCardRPC");
            discardPile.Add(id);
        }

        public void DiscardCard(int id) {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC("DiscardCardRPC", RpcTarget.AllBuffered, id);
            }
        }

        [PunRPC]
        void ReshuffleRPC() {
            Debug.Log(name + ".ReshuffleRPC");
            drawPile.AddRange(discardPile);
            discardPile.Clear();
        }

        [PunRPC]
        void DrawCardRPC(int id) {
            Debug.Log(name + ".DealCardRPC");
            drawPile.Remove(id);
        }

        public void Register(CardPlayer player) {
            Debug.Log(name + ".Register" + player.name);
            player.onRequestedCard += RequestCard;
            if (PhotonNetwork.IsMasterClient) {
                for (int i = 0; i < initialCards; i++) {
                    DealCard(player);
                }
            }
        }

        public void Unregister(CardPlayer player) {
            Debug.Log(name + ".Unregister" + player.name);
            player.onRequestedCard -= RequestCard;
            foreach (int card in player.cardsInHand) {
                DiscardCard(card);
            }
        }

        public void RequestCard(CardPlayer player) {
            Debug.Log(name + ".RequestCard");
            DealCard(player);
        }

        void DealCard(CardPlayer player) {
            Debug.Log(name + ".DealCard to" + player.name);
            if (PhotonNetwork.IsMasterClient) {
                if (drawPile.Count == 0) {
                    photonView.RPC("ReshuffleRPC", RpcTarget.All);
                }
                if (drawPile.Count > 0) {
                    float preferredPriority = Random.Range(1, 10);
                    var matchingCards = drawPile.Where(card => GetCardPriority(card) > preferredPriority).ToList();
                    if (matchingCards.Count > 0) {
                        int randomIndex = Random.Range(0, matchingCards.Count - 1);
                        int randomCard = drawPile[randomIndex];
                        photonView.RPC("DrawCardRPC", RpcTarget.AllBuffered, randomCard);
                        player.ReceiveCard(randomCard);
                    } else {
                        int randomIndex = Random.Range(0, drawPile.Count - 1);
                        int randomCard = drawPile[randomIndex];
                        photonView.RPC("DrawCardRPC", RpcTarget.AllBuffered, randomCard);
                        player.ReceiveCard(randomCard);
                    }
                }
            }
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
            Debug.Log(name + ".BuildDeck");
            drawPile = new List<int>();
            discardPile = new List<int>();
            for (int i = 0; i < CountryCardDatabase.instance.length; i++) {
                drawPile.Add(i);
            }
        }

        private void OnDestroy() {
            if (instance == this)
                instance = null;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        }
    }
}
