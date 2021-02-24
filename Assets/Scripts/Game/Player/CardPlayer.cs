using EQx.Game.Investing;
using EQx.Game.Statistics;
using EQx.Game.Table;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EQx.Game.Player {
    public enum PlayerState {
        Unregistered,
        Placing,
        Placed,
        Betting,
        Betted,
        Won,
        Lost
    }

    public class CardPlayer : MonoBehaviourPunCallbacks , IPunObservable {
        public static CardPlayer localPlayer = null;
        public static UnityAction<CardPlayer> localPlayerReady;

        public UnityAction<CardPlayer, string> onSetName;
        public UnityAction<CardPlayer, int> onSetAvatar;
        public UnityAction<CardPlayer> onRegister;
        public UnityAction<CardPlayer> onUnregister;
        public UnityAction<CardPlayer, int> onPlacedCard;
        public UnityAction<CardPlayer, int> onReceivedCard;
        public UnityAction<CardPlayer, int> onReceivedCoins;
        public UnityAction<CardPlayer, int> onInvestedCoins;
        public UnityAction<CardPlayer> onCommited;
        public UnityAction<CardPlayer> onPayedBlind;
        public UnityAction<CardPlayer> onRequestedCard;
        public UnityAction<CardPlayer> onStartedPlacing;
        public UnityAction<CardPlayer> onEndedPlacing;
        public UnityAction<CardPlayer> onStartedBetting;
        public UnityAction<CardPlayer> onEndedBetting;
        public UnityAction<CardPlayer> onWin;
        public UnityAction<CardPlayer> onLost;

        public string playerName;
        public int seatNumber = -1;
        public int avatarID = 0;

        public PlayerState state = PlayerState.Unregistered;

        public int placedCardID;
        public float baseValue;
        public float bonusValue;
        public float combinedValue => baseValue + bonusValue;

        public List<int> cardsInHand = new List<int>();

        [PunRPC]
        void RegisterRPC() {
            Debug.Log(name + ".RegisterRPC");
            CardDealer.instance.Register(this);
            InvestmentManager.instance.Register(this);
            PlayerObserver.instance.Register(this);
            RoundManager.instance.Register(this);
            onRegister?.Invoke(this);
        }

        [PunRPC]
        void UnregisterRPC() {
            Debug.Log(name + ".UnregisterRPC");
            state = PlayerState.Unregistered;
            CardDealer.instance.Unregister(this);
            InvestmentManager.instance.Unregister(this);
            PlayerObserver.instance.Unregister(this);
            RoundManager.instance.Unregister(this);
            onUnregister?.Invoke(this);
        }

        [PunRPC]
        void StartPlacingRPC() {
            Debug.Log(name + ".StartPlacingRPC");
            state = PlayerState.Placing;
            onStartedPlacing?.Invoke(this);
        }

        [PunRPC]
        void EndPlacingRPC() {
            Debug.Log(name + ".EndPlacingRPC");
            state = PlayerState.Placed;
            onEndedPlacing?.Invoke(this);
        }

        [PunRPC]
        void StartBettingRPC() {
            Debug.Log(name + ".StartBettingRPC");
            state = PlayerState.Betting;
            onStartedBetting?.Invoke(this);
        }

        [PunRPC]
        void EndBettingRPC() {
            Debug.Log(name + ".EndBettingRPC");
            state = PlayerState.Betted;
            onEndedBetting?.Invoke(this);
        }

        [PunRPC]
        void WinRPC() {
            Debug.Log(name + ".WinRPC");
            state = PlayerState.Won;
            onWin?.Invoke(this);
        }

        [PunRPC]
        void LoseRPC() {
            Debug.Log(name + ".LoseRPC");
            state = PlayerState.Lost;
            onLost?.Invoke(this);
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
            placedCardID = id;
            cardsInHand.Remove(id);
            onPlacedCard?.Invoke(this, id);
        }

        [PunRPC]
        void SetNameRPC(string newName) {
            Debug.Log(name+".SetNameRPC");
            playerName = newName;
            onSetName?.Invoke(this, newName);
        }

        [PunRPC]
        void SetAvatarRPC(int id) {
            Debug.Log(name + ".SetAvatarRPC");
            avatarID = id;
            onSetAvatar?.Invoke(this, avatarID);
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
        public void StartPlacing() {
            Debug.Log(name + ".StartPlacing");
            if (photonView.IsMine) {
                photonView.RPC("StartPlacingRPC", RpcTarget.AllBufferedViaServer);
            }
        }

        public void EndPlacing() {
            Debug.Log(name + ".EndPlacing");
            if (photonView.IsMine) {
                photonView.RPC("EndPlacingRPC", RpcTarget.AllBufferedViaServer);
            }
        }

        public void StartBetting() {
            Debug.Log(name + ".StartBetting");
            if (photonView.IsMine) {
                photonView.RPC("StartBettingRPC", RpcTarget.AllBufferedViaServer);
            }
        }

        public void EndBetting() {
            Debug.Log(name + ".EndBetting");
            if (photonView.IsMine) {
                photonView.RPC("EndBettingRPC", RpcTarget.AllBufferedViaServer);
            }
        }

        public void Win() {
            Debug.Log(name + ".Win");
            if (photonView.IsMine) {
                photonView.RPC("WinRPC", RpcTarget.AllBufferedViaServer);
            }
        }

        public void Lose() {
            Debug.Log(name + ".Lose");
            if (photonView.IsMine) {
                photonView.RPC("LoseRPC", RpcTarget.AllBufferedViaServer);
            }
        }

        public void RequestCard() {
            Debug.Log(name + ".RequestCard");
            if (photonView.IsMine) {
                photonView.RPC("RequestCardRPC", RpcTarget.AllBufferedViaServer);
            }
        }

        public void PlaceCard(int id) {
            Debug.Log(name + ".PlaceCard");
            if (photonView.IsMine && state == PlayerState.Placing) {
                photonView.RPC("PlaceCardRPC", RpcTarget.AllBufferedViaServer, id);
            }
        }

        public void InvestCoins(int amount) {
            Debug.Log(name + ".InvestCoins" + amount);
            if (photonView.IsMine && state == PlayerState.Betting) {
                photonView.RPC("InvestCoinsRPC", RpcTarget.AllBufferedViaServer, amount);
            }
        }

        public void PayBlind() {
            Debug.Log(name + ".PayBlind");
            if (photonView.IsMine) {
                photonView.RPC("PayBlindRPC", RpcTarget.AllBufferedViaServer);
            }
        }

        public void Commit() {
            Debug.Log(name + ".Commit");
            if (photonView.IsMine) {
                photonView.RPC("CommitRPC", RpcTarget.AllBufferedViaServer);
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
            state = PlayerState.Unregistered;
            if (photonView.IsMine) {
                localPlayer = this;
                localPlayerReady?.Invoke(this);
                photonView.RPC("SetNameRPC", RpcTarget.AllBuffered, PlayerPrefs.GetString(PlayerPrefKeys.PLAYERNAME, "Anonymous"));
                photonView.RPC("SetAvatarRPC", RpcTarget.AllBuffered, PlayerPrefs.GetInt(PlayerPrefKeys.PLAYER_AVATAR));
                photonView.RPC("RegisterRPC", RpcTarget.AllBufferedViaServer);
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
