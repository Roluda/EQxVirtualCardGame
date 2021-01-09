using EQx.Game.Investing;
using EQx.Game.Table;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EQx.Game.Player {
    public class CardPlayer : MonoBehaviourPunCallbacks , IPunObservable {
        public static CardPlayer localPlayer = null;
        public static UnityAction<CardPlayer> localPlayerReady;

        public UnityAction<CardPlayer> onRegister;
        public UnityAction<CardPlayer> onUnregister;
        public UnityAction<CardPlayer, int> onPlacedCard;
        public UnityAction<CardPlayer, int> onReceivedCard;
        public UnityAction<CardPlayer, int> onReceivedCoins;
        public UnityAction<CardPlayer, int> onInvestedCoins;
        public UnityAction<CardPlayer, int> onExtractedCoins;
        public UnityAction<CardPlayer> onCommited;
        public UnityAction<CardPlayer> onPayedBlind;
        public UnityAction<CardPlayer, string> onSetName;
        public UnityAction<CardPlayer> onRequestedCard;
        public UnityAction<CardPlayer> onEndedTurn;
        public UnityAction<CardPlayer> onStartedTurn;

        public string playerName;
        public int seatNumber = -1;
        public bool onTurn = false;
        public bool cardPlaced = false;
        public bool lost = false;
        public List<int> cardsInHand = new List<int>();

        [PunRPC]
        void RegisterRPC() {
            Debug.Log(name + ".RegisterRPC");
            CardDealer.instance.Register(this);
            InvestmentManager.instance.Register(this);
            RoundManager.instance.Register(this);
            onRegister?.Invoke(this);
        }

        [PunRPC]
        void UnregisterRPC() {
            Debug.Log(name + ".UnregisterRPC");
            CardDealer.instance.Unregister(this);
            InvestmentManager.instance.Unregister(this);
            RoundManager.instance.Unregister(this);
            onUnregister?.Invoke(this);
        }

        [PunRPC]
        void StartTurnRPC() {
            Debug.Log(name + ".StartTurnRPC");
            onTurn = true;
            onStartedTurn?.Invoke(this);
        }

        [PunRPC]
        void EndTurnRPC() {
            Debug.Log(name + ".EndTurnRPC");
            onTurn = false;
            onEndedTurn?.Invoke(this);
        }

        [PunRPC]
        void RequestCardRPC() {
            Debug.Log(name + ".RequestCardRPC");
            onRequestedCard?.Invoke(this);
        }

        [PunRPC]
        void ReceiveCardRPC(int id) {
            Debug.Log(name + ".ReceiveCardRPC");
            cardsInHand.Add(id);
            onReceivedCard?.Invoke(this, id);
        }

        [PunRPC]
        void PlaceCardRPC(int id) {
            Debug.Log(name + ".PlaceCardRPC");
            cardPlaced = true;
            cardsInHand.Remove(id);
            onPlacedCard?.Invoke(this, id);
        }

        [PunRPC]
        void SetNameRPC(string newName) {
            Debug.Log(newName + ".SetNameRPC");
            playerName = newName;
            name = newName;
            onSetName?.Invoke(this, newName);
        }

        [PunRPC]
        void ReceiveCoinsRPC(int amount) {
            Debug.Log(name + ".ReceiveCoinsRPC" + amount);
            onReceivedCoins?.Invoke(this, amount);
        }

        [PunRPC]
        void InvestCoinsRPC(int amount) {
            Debug.Log(name + ".InvestCoinsRPC" + amount);
            onInvestedCoins?.Invoke(this, amount);
        }

        [PunRPC]
        void ExtractCoinsRPC(int amount) {
            Debug.Log(name + ".ExtractCoinsRPC" + amount);
            onExtractedCoins?.Invoke(this, amount);
        }

        [PunRPC]
        void PayBlindRPC(){
            Debug.Log(name + ".PayBlindRPC");
            onPayedBlind?.Invoke(this);
        }

        [PunRPC]
        void CommitRPC() {
            Debug.Log(name + ".CommitRPC");
            onCommited?.Invoke(this);
        }


        //Client Side RPCs
        public void StartTurn() {
            Debug.Log(name + ".StartTurn");
            if (photonView.IsMine) {
                photonView.RPC("StartTurnRPC", RpcTarget.AllBuffered);
            }
        }

        public void EndTurn() {
            Debug.Log(name + ".EndTurn");
            if (photonView.IsMine) {
                photonView.RPC("EndTurnRPC", RpcTarget.AllBuffered);
            }
        }

        public void RequestCard() {
            Debug.Log(name + ".RequestCard");
            if (photonView.IsMine) {
                photonView.RPC("RequestCardRPC", RpcTarget.AllBuffered);
            }
        }

        public void PlaceCard(int id) {
            Debug.Log(name + ".PlaceCard");
            if (photonView.IsMine && onTurn && !cardPlaced) {
                photonView.RPC("PlaceCardRPC", RpcTarget.AllBuffered, id);
            }
        }

        public void InvestCoins(int amount) {
            Debug.Log(name + ".InvestCoins" + amount);
            if (photonView.IsMine) {
                photonView.RPC("InvestCoinsRPC", RpcTarget.AllBuffered, amount);
            }
        }

        public void ExtractCoins(int amount) {
            Debug.Log(name + ".ExtractCoins" + amount);
            if (photonView.IsMine) {
                photonView.RPC("ExtractCoinsRPC", RpcTarget.AllBuffered, amount);
            }
        }

        public void PayBlind() {
            Debug.Log(name + ".PayBlind");
            if (photonView.IsMine) {
                photonView.RPC("PayBlindRPC", RpcTarget.AllBuffered);
            }
        }

        public void Commit() {
            Debug.Log(name + ".Commit");
            if (photonView.IsMine) {
                photonView.RPC("CommitRPC", RpcTarget.AllBuffered);
            }
        }


        //Server Side RPCs
        public void ReceiveCard(int id) {
            Debug.Log(name + ".ReceiveCard: "+id);
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC("ReceiveCardRPC", RpcTarget.AllBuffered, id);
            }
        }

        public void ReceiveCoins(int amount) {
            Debug.Log(name + ".ReceiveCoins: " + amount);
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC("ReceiveCoinsRPC", RpcTarget.AllBuffered, amount);
            }
        }

        public void Unregister() {
            Debug.Log(name + ".Unregister");
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC("UnregisterRPC", RpcTarget.AllBuffered);
            }
        }



        void Start() {
            Debug.Log(name + ".Start");
            if (photonView.IsMine) {
                localPlayer = this;
                localPlayerReady?.Invoke(this);
                photonView.RPC("SetNameRPC", RpcTarget.AllBuffered, PlayerPrefs.GetString(PlayerPrefKeys.PLAYERNAME, "Anonymous"));
                photonView.RPC("RegisterRPC", RpcTarget.AllBuffered);
            }
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) {
            Debug.Log(name + ".OnPlayerLeftRoom");
            if (photonView.Owner == otherPlayer) {
                RoundManager.instance.Unregister(this);
                CardDealer.instance.Unregister(this);
                InvestmentManager.instance.Unregister(this);
                Destroy(this);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        }
    }
}
