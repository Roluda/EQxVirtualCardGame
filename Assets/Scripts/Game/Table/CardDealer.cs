using EQx.Game.CountryCards;
using EQx.Game.Player;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Table {
    public class CardDealer : MonoBehaviourPunCallbacks {

        public static CardDealer instance = null;

        List<int> drawPile;
        List<int> discardPile;

        [PunRPC]
        void DiscardCardRPC(int id) {
            Debug.Log(name + "DiscardCardRPC");
            discardPile.Add(id);
        }

        public void DiscardCard(int id) {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC("DiscardCardRPC", RpcTarget.AllBuffered, id);
            }
        }

        [PunRPC]
        void ReshuffleRPC() {
            Debug.Log(name + "ReshuffleRPC");
            drawPile.AddRange(discardPile);
            discardPile.Clear();
        }

        [PunRPC]
        void DrawCardRPC(int id) {
            Debug.Log(name + "DealCardRPC");
            drawPile.Remove(id);
        }

        public void Register(CardPlayer player) {
            player.onRequestedCard += RequestCard;
        }

        public void Unregister(CardPlayer player) {
            player.onRequestedCard -= RequestCard;
        }

        public void RequestCard(CardPlayer player) {
            Debug.Log(name + "RequestCard");
            
            if (PhotonNetwork.IsMasterClient) {
                if(drawPile.Count == 0) {
                    photonView.RPC("ReshuffleRPC", RpcTarget.All);
                }
                if (drawPile.Count > 0) {
                    int randomIndex = Random.Range(0, drawPile.Count - 1);
                    int randomCard = drawPile[randomIndex];
                    photonView.RPC("DrawCardRPC", RpcTarget.AllBuffered, randomCard);
                    player.ReceiveCard(randomCard);
                }
            }
        }

        private void Awake() {
            if(instance != null) {
                Destroy(gameObject);
            } else {
                instance = this;
            }
        }

        // Start is called before the first frame update
        void Start() {
            BuildDeck();
        }

        void BuildDeck() {
            Debug.Log(name + "BuildDeck");
            drawPile = new List<int>();
            discardPile = new List<int>();
            foreach(var card in CountryCardDatabase.instance.data) {
                drawPile.Add(card.cardID);
            }
        }

        private void OnDestroy() {
            if (instance == this)
                instance = null;
        }
    }
}
