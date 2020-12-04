using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using EQx;
using EQx.Game.Table;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace EQx.Game.Player {
    public class CardPlayer : CardPlayerBehavior {
        public static CardPlayer localPlayer = null;
        public static UnityAction<CardPlayer> localPlayerReady;
        [SerializeField]
        int cardPlacingLimit = 1;

        public UnityAction<CardPlayer, int> onPlacedCard;
        public UnityAction<CardPlayer, int> onReceivedCard;
        public UnityAction<CardPlayer, string> onSetName;
        public UnityAction<CardPlayer> onRequestedCard;
        public UnityAction<CardPlayer> onEndedTurn;
        public UnityAction<CardPlayer> onStartedTurn;
        public UnityAction<CardPlayer> onWinRound;

        public string playerName;

        bool onTurn = false;
        int cardsPlacedThisTurn = 0;

        public override void StartTurn(RpcArgs args) {
            Debug.Log(name + "StartTurnRPC");
            cardsPlacedThisTurn = 0;
            onTurn = true;
            onStartedTurn?.Invoke(this);
        }

        public override void EndTurn(RpcArgs args) {
            Debug.Log(name + "EndTurnRPC");
            onTurn = false;
            onEndedTurn?.Invoke(this);
        }

        public override void RequestCard(RpcArgs args) {
            Debug.Log(name + "RequestCardRPC");
            onRequestedCard?.Invoke(this);
        }

        public override void ReceiveCard(RpcArgs args) {
            Debug.Log(name + "ReceiveCardRPC");
            onReceivedCard?.Invoke(this, args.GetNext<int>());
        }

        public override void PlaceCard(RpcArgs args) {
            Debug.Log(name + "PlaceCardRPC");
            cardsPlacedThisTurn++;
            onPlacedCard?.Invoke(this, args.GetNext<int>());
        }

        public override void SetName(RpcArgs args) {
            Debug.Log(name + "SetNameRPC");
            playerName = args.GetNext<string>();
            name = playerName;
            onSetName?.Invoke(this, playerName);
        }

        public override void WinRound(RpcArgs args) {
            Debug.Log(name + "WinRoundRPC");
            onWinRound?.Invoke(this);
        }




        public void CallStartTurn() {
            Debug.Log(name + "StartTurn");
            if (networkObject.IsOwner) {
                networkObject.SendRpc(RPC_START_TURN, Receivers.AllBuffered);
            }
        }

        public void CallEndTurn() {
            Debug.Log(name + "EndTurn");
            if (networkObject.IsOwner && onTurn) {
                networkObject.SendRpc(RPC_END_TURN, Receivers.AllBuffered);
            }
        }

        public void CallRequestCard() {
            Debug.Log(name + "RequestCard");
            if (networkObject.IsOwner) {
                networkObject.SendRpc(RPC_REQUEST_CARD, Receivers.AllBuffered);
            }
        }

        public void CallReceiveCard(int id) {
            Debug.Log(name + "ReceiveCard");
            networkObject.SendRpc(RPC_RECEIVE_CARD, Receivers.AllBuffered, id);
        }

        public void CallPlaceCard(int id) {
            Debug.Log(name + "PlaceCard");
            if (networkObject.IsOwner && onTurn && cardsPlacedThisTurn<cardPlacingLimit) {
                networkObject.SendRpc(RPC_PLACE_CARD, Receivers.AllBuffered, id);
            }
        }

        public void CallWinRound() {
            Debug.Log(name + "CallWinRound");
            if (networkObject.IsOwner) {
                networkObject.SendRpc(RPC_WIN_ROUND, Receivers.AllBuffered);
            }
        }


        protected override void NetworkStart() {
            base.NetworkStart();
            Debug.Log(name + "NetworkStart");
            GameTable.instance.TakeSeat(this);
            if (networkObject.IsOwner) {
                networkObject.SendRpc(RPC_SET_NAME, Receivers.AllBuffered, PlayerPrefs.GetString(PlayerPrefKeys.PLAYERNAME, "Anonymous"));
                localPlayer = this;
                localPlayerReady?.Invoke(this);
                FindObjectOfType<Hand>().Initialize(this);
            }
            networkObject.Owner.disconnected += DisconnectedListener;
        }

        void DisconnectedListener(NetWorker sender) {
            Debug.Log(name + "DisconnectedListener");
            GameTable.instance.LeaveTable(this);
            if (networkObject.IsOwner) {
                localPlayer = null;
                SceneManager.LoadScene(0);
            }
            networkObject.Destroy();
        }


    }
}
