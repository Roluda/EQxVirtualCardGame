using EQx.Game.Table;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace EQx.Game.Player {
    public class CardPlayer : MonoBehaviourPunCallbacks , IPunObservable {
        public static CardPlayer localPlayer = null;
        public static UnityAction<CardPlayer> localPlayerReady;
        [SerializeField]
        int cardPlacingLimit = 1;

        public UnityAction<CardPlayer> onRegister;
        public UnityAction<CardPlayer, int> onPlacedCard;
        public UnityAction<CardPlayer, int> onReceivedCard;
        public UnityAction<CardPlayer, string> onSetName;
        public UnityAction<CardPlayer> onRequestedCard;
        public UnityAction<CardPlayer> onEndedTurn;
        public UnityAction<CardPlayer> onStartedTurn;
        public UnityAction<CardPlayer> onWinRound;

        public string playerName;
        public int seatNumber = -1;
        public bool onTurn = false;
        int cardsPlacedThisTurn = 0;

        [PunRPC]
        void RegisterRPC() {
            Debug.Log(name + ".RegisterRPC");
            RoundManager.instance.Register(this);
            CardDealer.instance.Register(this);
        }

        [PunRPC]
        void StartTurnRPC() {
            Debug.Log(name + ".StartTurnRPC");
            cardsPlacedThisTurn = 0;
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
            onReceivedCard?.Invoke(this, id);
        }

        [PunRPC]
        void PlaceCardRPC(int id) {
            Debug.Log(name + ".PlaceCardRPC");
            cardsPlacedThisTurn++;
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
        void WinRoundRPC() {
            Debug.Log(name + ".WinRoundRPC");
            onWinRound?.Invoke(this);
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
            if (photonView.IsMine && onTurn && cardsPlacedThisTurn < cardPlacingLimit) {
                photonView.RPC("PlaceCardRPC", RpcTarget.AllBuffered, id);
            }
        }

        public void CallWinRound() {
            Debug.Log(name + ".CallWinRound");
            if (photonView.IsMine) {
                photonView.RPC("WinRoundRPC", RpcTarget.AllBuffered);
            }
        }


        public void ReceiveCard(int id) {
            Debug.Log(name + ".ReceiveCard");
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC("ReceiveCardRPC", RpcTarget.AllBuffered, id);
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
                Destroy(gameObject);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        }
    }
}
