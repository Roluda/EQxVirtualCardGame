using EQx.Game.Investing;
using EQx.Game.Statistics;
using EQx.Game.Table;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EQx.Game.Player {
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
        public UnityAction<CardPlayer, int> onStartedPlacing;
        public UnityAction<CardPlayer, int> onEndedPlacing;
        public UnityAction<CardPlayer, int> onStartedBetting;
        public UnityAction<CardPlayer, int> onEndedBetting;
        public UnityAction<CardPlayer> onCommited;
        public UnityAction<CardPlayer> onPayedBlind;
        public UnityAction<CardPlayer> onRequestedCard;
        public UnityAction<CardPlayer> onWin;
        public UnityAction<CardPlayer> onLost;

        public string playerName;
        public int avatarID = 0;

        [PunRPC]
        void RegisterRPC() {
            Logger.Log($"{name}.{nameof(RegisterRPC)}");
            PlayerObserver.instance.Register(this);
            CardDealer.instance.Register(this);
            InvestmentManager.instance.Register(this);
            RoundManager.instance.Register(this);
            onRegister?.Invoke(this);
        }

        [PunRPC]
        void UnregisterRPC() {
            Logger.Log($"{name}.{nameof(UnregisterRPC)}");
            PlayerObserver.instance.Unregister(this);
            CardDealer.instance.Unregister(this);
            InvestmentManager.instance.Unregister(this);
            RoundManager.instance.Unregister(this);
            onUnregister?.Invoke(this);
        }

        [PunRPC]
        void StartPlacingRPC(int round) {
            Logger.Log($"{name}.{nameof(StartPlacingRPC)},({round})");
            onStartedPlacing?.Invoke(this, round);
        }

        [PunRPC]
        void EndPlacingRPC(int round) {
            Logger.Log($"{name}.{nameof(EndPlacingRPC)}({round})");
            onEndedPlacing?.Invoke(this, round);
        }

        [PunRPC]
        void StartBettingRPC(int round) {
            Logger.Log($"{name}.{nameof(StartBettingRPC)}({round})");
            onStartedBetting?.Invoke(this, round);
        }

        [PunRPC]
        void EndBettingRPC(int round) {
            Logger.Log($"{name}.{nameof(EndBettingRPC)}({round})");
            onEndedBetting?.Invoke(this, round);
        }

        [PunRPC]
        void WinRPC() {
            Logger.Log($"{name}.{nameof(WinRPC)}");
            onWin?.Invoke(this);
        }

        [PunRPC]
        void LoseRPC() {
            Logger.Log($"{name}.{nameof(LoseRPC)}");
            onLost?.Invoke(this);
        }

        [PunRPC]
        void RequestCardRPC() {
            Logger.Log($"{name}.{nameof(RequestCardRPC)}");
            onRequestedCard?.Invoke(this);
        }

        [PunRPC]
        void ReceiveCardRPC(int id) {
            Logger.Log($"{name}.{nameof(ReceiveCardRPC)}({id})");
            onReceivedCard?.Invoke(this, id);
        }

        [PunRPC]
        void PlaceCardRPC(int id) {
            Logger.Log($"{name}.{nameof(PlaceCardRPC)}({id})");
            onPlacedCard?.Invoke(this, id);
        }

        [PunRPC]
        void SetNameRPC(string newName) {
            Logger.Log($"{name}.{nameof(SetNameRPC)}({newName})");
            name = newName;
            playerName = newName;
            onSetName?.Invoke(this, newName);
        }

        [PunRPC]
        void SetAvatarRPC(int id) {
            Logger.Log($"{name}.{nameof(SetAvatarRPC)}({id})");
            avatarID = id;
            onSetAvatar?.Invoke(this, avatarID);
        }

        [PunRPC]
        void ReceiveCoinsRPC(int amount) {
            Logger.Log($"{name}.{nameof(ReceiveCoinsRPC)}({amount})");
            onReceivedCoins?.Invoke(this, amount);
        }

        [PunRPC]
        void InvestCoinsRPC(int amount) {
            Logger.Log($"{name}.{nameof(InvestCoinsRPC)}({amount})");
            onInvestedCoins?.Invoke(this, amount);
        }

        [PunRPC]
        void PayBlindRPC(){
            Logger.Log($"{name}.{nameof(PayBlindRPC)}");
            onPayedBlind?.Invoke(this);
        }

        [PunRPC]
        void CommitRPC() {
            Logger.Log($"{name}.{nameof(CommitRPC)}");
            onCommited?.Invoke(this);
        }

        //Client Side RPCs
        public void PlaceCard(int id) {
            if (photonView.IsMine) {
                photonView.RPC(nameof(PlaceCardRPC), RpcTarget.AllBufferedViaServer, id);
            }
        }

        public void InvestCoins(int amount) {
            if (photonView.IsMine) {
                photonView.RPC(nameof(InvestCoinsRPC), RpcTarget.AllBufferedViaServer, amount);
            }
        }

        public void Commit() {
            if (photonView.IsMine) {
                photonView.RPC(nameof(CommitRPC), RpcTarget.AllBufferedViaServer);
            }
        }


        //Server Side RPCs
        public void StartPlacing(int round) {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC(nameof(StartPlacingRPC), RpcTarget.AllBufferedViaServer, round);
            }
        }

        public void EndPlacing(int round) {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC(nameof(EndPlacingRPC), RpcTarget.AllBufferedViaServer, round);
            }
        }

        public void StartBetting(int round) {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC(nameof(StartBettingRPC), RpcTarget.AllBufferedViaServer, round);
            }
        }

        public void EndBetting(int round) {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC(nameof(EndBettingRPC), RpcTarget.AllBufferedViaServer, round);
            }
        }

        public void Win() {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC(nameof(WinRPC), RpcTarget.AllBufferedViaServer);
            }
        }

        public void Lose() {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC(nameof(LoseRPC), RpcTarget.AllBufferedViaServer);
            }
        }

        public void PayBlind() {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC(nameof(PayBlindRPC), RpcTarget.AllBufferedViaServer);
            }
        }

        public void RequestCard() {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC(nameof(RequestCardRPC), RpcTarget.AllBufferedViaServer);
            }
        }

        public void ReceiveCard(int id) {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC(nameof(ReceiveCardRPC), RpcTarget.AllBuffered, id);
            }
        }

        public void ReceiveCoins(int amount) {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC(nameof(ReceiveCoinsRPC), RpcTarget.AllBuffered, amount);
            }
        }

        public void Unregister() {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC(nameof(UnregisterRPC), RpcTarget.AllBuffered);
            }
        }

        private void Awake() {
            if (photonView.IsMine) {
                localPlayer = this;
                localPlayerReady?.Invoke(this);
                photonView.RPC("SetNameRPC", RpcTarget.AllBufferedViaServer, PlayerPrefs.GetString(PlayerPrefKeys.PLAYERNAME, "Anonymous"));
                photonView.RPC("SetAvatarRPC", RpcTarget.AllBufferedViaServer, PlayerPrefs.GetInt(PlayerPrefKeys.PLAYER_AVATAR));
                photonView.RPC("RegisterRPC", RpcTarget.AllBufferedViaServer);
            }
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) {
            Logger.Log($"{name}.{nameof(OnPlayerLeftRoom)}({otherPlayer.UserId})");
            if (photonView.Owner == otherPlayer) {
                UnregisterRPC();
                Destroy(this);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        }
    }
}
