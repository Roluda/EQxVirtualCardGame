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

        [SerializeField]
        int cardPlacingLimit = 1;

        public UnityAction<CardPlayer, int> onPlacedCard;
        public UnityAction<CardPlayer, int> onReceivedCard;
        public UnityAction<CardPlayer> onRequestedCard;
        public UnityAction<CardPlayer> onEndedTurn;
        public UnityAction<CardPlayer> onStartedTurn;

        public string playerName;

        int cardsPlacedThisTurn = 0;

        public override void StartTurn(RpcArgs args) {
            MainThreadManager.Run(() => {
                Debug.Log(name + "StartTurnRPC");
                onStartedTurn?.Invoke(this);
            });
        }

        public override void EndTurn(RpcArgs args) {
            MainThreadManager.Run(() => {
                Debug.Log(name + "EndTurnRPC");
                onEndedTurn?.Invoke(this);
            });
        }

        public override void RequestCard(RpcArgs args) {
            MainThreadManager.Run(() => {
                Debug.Log(name + "RequestCardRPC");
                onRequestedCard?.Invoke(this);
            });
        }

        public override void ReceiveCard(RpcArgs args) {
            MainThreadManager.Run(() => {
                Debug.Log(name + "ReceiveCardRPC");
                onReceivedCard?.Invoke(this, args.GetNext<int>());
            });
        }

        public override void PlaceCard(RpcArgs args) {
            MainThreadManager.Run(() => {
                Debug.Log(name + "PlaceCardRPC");
                cardsPlacedThisTurn++;
                onPlacedCard?.Invoke(this, args.GetNext<int>());
            });
        }

        public override void SetName(RpcArgs args) {
            MainThreadManager.Run(() => {
                Debug.Log(name + "SetNameRPC");
                playerName = args.GetNext<string>();
            });
        }




        public void CallStartTurn() {
            Debug.Log(name + "StartTurn");
            if (networkObject.IsOwner) {
                networkObject.SendRpc(RPC_START_TURN, Receivers.AllBuffered);
            }
        }

        public void CallEndTurn() {
            Debug.Log(name + "EndTurn");
            if (networkObject.IsOwner) {
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
            if (networkObject.IsOwner && cardsPlacedThisTurn<cardPlacingLimit) {
                networkObject.SendRpc(RPC_PLACE_CARD, Receivers.AllBuffered, id);
            }
        }



        protected override void NetworkStart() {
            Debug.Log(name + "NetworkStart");
            GameTable.instance.TakeSeat(this);
            if (networkObject.IsOwner) {
                networkObject.SendRpc(RPC_SET_NAME, Receivers.AllBuffered, PlayerPrefs.GetString(PlayerPrefKeys.PLAYERNAME, "Anonymous"));
                FindObjectOfType<Hand>().Initialize(this);
            }

            networkObject.Owner.disconnected += DisconnectedListener;
        }

        void DisconnectedListener(NetWorker sender) {
            Debug.Log(name + "DisconnectedListener");
            GameTable.instance.LeaveTable(this);
            Destroy(this);
            if (networkObject.IsOwner) {
                SceneManager.LoadScene(0);
            }
        }
    }
}
