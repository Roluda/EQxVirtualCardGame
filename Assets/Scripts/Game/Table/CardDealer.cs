using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using EQx.Game.CountryCards;
using EQx.Game.Player;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Table {
    public class CardDealer : CardDealerBehavior {

        public List<int> drawPile;
        public List<int> discardPile;

        public override void DiscardCard(RpcArgs args) {
            Debug.Log(name + "DiscardCardRPC");
            discardPile.Add(args.GetNext<int>());
        }

        public void CallDiscardCard(int id) {
            if (networkObject.IsOwner) {
                networkObject.SendRpc(RPC_DISCARD_CARD, Receivers.AllBuffered, id);
            }
        }

        public override void Reshuffle(RpcArgs args) {
            Debug.Log(name + "ReshuffleRPC");
            drawPile.AddRange(discardPile);
            discardPile.Clear();
        }

        public override void DealCard(RpcArgs args) {
            Debug.Log(name + "DealCardRPC");
            drawPile.Remove(args.GetNext<int>());
        }

        public void RequestCard(CardPlayer player) {
            Debug.Log(name + "RequestCard");
            if(networkObject ==null) {
                Debug.LogWarning("NetworkObject Null");
                return;
            }
            if (networkObject.IsServer) {
                if(drawPile.Count == 0) {
                    networkObject.SendRpc(RPC_RESHUFFLE, Receivers.OthersBuffered);
                    drawPile.AddRange(discardPile);
                    discardPile.Clear();
                }
                if (drawPile.Count > 0) {
                    int randomIndex = Random.Range(0, drawPile.Count - 1);
                    int randomCard = drawPile[randomIndex];
                    player.CallReceiveCard(randomCard);
                    networkObject.SendRpc(RPC_DEAL_CARD, Receivers.OthersBuffered, randomCard);
                    drawPile.RemoveAt(randomIndex);
                }
            }
        }

        protected override void NetworkStart() {
            base.NetworkStart();
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

        // Update is called once per frame
        void Update() {

        }


    }
}
