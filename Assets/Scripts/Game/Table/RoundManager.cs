using EQx.Game.CountryCards;
using EQx.Game.Investing;
using EQx.Game.Player;
using EQx.Menu;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

namespace EQx.Game.Table {
    public class RoundManager : MonoBehaviourPunCallbacks , IPunObservable {
        public static RoundManager instance = null;

        [SerializeField]
        RoundData defaultRound = default;
        [SerializeField]
        RoundData[] scriptedRounds = default;
        [SerializeField]
        public int maxRounds;

        public UnityAction<EQxVariableType> onNewDemand;
        public UnityAction<CardPlayer> onPlayerRegister;
        public UnityAction<CardPlayer> onPlayerUnregister;
        public UnityAction onRegisterUpdate;
        public UnityAction onPlacingEnded;
        public UnityAction onPlacingStarted;
        public UnityAction onBettingEnded;
        public UnityAction onBettingStarted;
        public UnityAction onNewRound;
        public UnityAction onGameEnd;

        List<RoundParticipant> participants = new List<RoundParticipant>();
        public EQxVariableType currentDemand;

        public int currentRound = 0;
        bool inRound = false;
        bool gameOver = false;

        public void Register(CardPlayer player) {
            player.onPlacedCard += PlacedCardListener;
            player.onCommited += CommittedListener;
            player.onStartedPlacing += StartPlacingListener;
            player.onStartedBetting += StartBettingListener;

            string userID = player.photonView.Owner.UserId;
            var participant = participants.FirstOrDefault(part => part.userID == userID);
            if (participant!=null) {
                participant.player = player;
                if (PhotonNetwork.IsMasterClient) {
                    RecoverState(participant.userID, (int)participant.state, participant.placedCardID, participant.baseValue, participant.bonusValue);
                }
            } else {
                participant = new RoundParticipant() {
                    player = player,
                    userID = userID,
                    seatNumber = -2,
                    state = inRound ? RoundState.Start : RoundState.Waiting
                };
                
                participants.Add(participant);
            }
            participant.active = true;
            onPlayerRegister?.Invoke(player);
            var orderedParticipants = AllActiveParticipants().OrderBy(part => part.actorNumber).ToList();
            for(int i = 0; i < orderedParticipants.Count; i++) {
                orderedParticipants[i].seatNumber = i;
            }
            onRegisterUpdate?.Invoke();
            if (PhotonNetwork.IsMasterClient) {
                if (participants.Count() == 1) {
                    NewRound();
                } else {
                    DetermineNextAction(participant);
                }
            }
            UpdateRoomProperties();
        }

        public void Unregister(CardPlayer player) {
            player.onPlacedCard -= PlacedCardListener;
            player.onCommited -= CommittedListener;
            player.onStartedPlacing -= StartPlacingListener;
            player.onStartedBetting -= StartBettingListener;
            GetParticipant(player).active = false;
            onPlayerUnregister?.Invoke(player);
            onRegisterUpdate?.Invoke();
            TryEndRound();
            UpdateRoomProperties();
        }


        private void DetermineNextAction(RoundParticipant participant) {
            if (!participant.active) {
                return;
            }
            switch (participant.state) {
                case RoundState.Waiting:
                    break;
                case RoundState.Start:
                    HandlePreviousCard(participant);
                    participant.Reset();
                    participant.player.StartPlacing(currentRound);
                    break;
                case RoundState.Placing:
                    participant.player.StartPlacing(currentRound);
                    break;
                case RoundState.Placed:
                    participant.player.EndPlacing(currentRound);
                    participant.player.PayBlind();
                    participant.player.StartBetting(currentRound);
                    break;
                case RoundState.Betting:
                    participant.player.PayBlind();
                    participant.player.StartBetting(currentRound);
                    break;
                case RoundState.Betted:
                    participant.player.EndBetting(currentRound);
                    TryEndRound();
                    break;
                case RoundState.Won:
                    participant.player.Win();
                    break;
                case RoundState.Lost:
                    participant.player.Lose();
                    break;
                default:
                    throw new NotImplementedException("This Round State does not exist");

            }
        }

        private void HandlePreviousCard(RoundParticipant participant) {
            if(participant.placedCardID != -1) {
                CardDealer.instance.DiscardCard(participant.placedCardID);
                participant.player.RequestCard();
            }
        }



        private void StartPlacingListener(CardPlayer player, int round) {
            GetParticipant(player).state = RoundState.Placing;
        }

        private void StartBettingListener(CardPlayer player, int round) {
            GetParticipant(player).state = RoundState.Betting;
        }

        private void CommittedListener(CardPlayer player) {
            var participant = GetParticipant(player);
            if(!(participant.state == RoundState.Betting)) {
                return;
            }
            participant.bonusValue = InvestmentManager.instance.BonusValue(player);
            participant.state = RoundState.Betted;
            DetermineNextAction(GetParticipant(player));
        }

        void PlacedCardListener(CardPlayer player, int id) {
            var participant = GetParticipant(player);
            if (!(participant.state == RoundState.Placing)) {
                return;
            }
            participant.placedCardID = id;
            participant.baseValue = CountryCardDatabase.instance.GetCountry(id).GetValue(currentDemand);
            participant.state = RoundState.Placed;
            DetermineNextAction(GetParticipant(player));
        }

        void RecoverState(string userID, int state, int placedCard, float baseValue, float bonusValue) {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC(nameof(RecoverStateRPC), RpcTarget.AllBuffered, userID, state, placedCard, baseValue, bonusValue);
            }
        }

        [PunRPC]
        private void RecoverStateRPC(string userID, int state, int placedCard, float baseValue, float bonusValue) {
            Logger.Log($"{name}.{nameof(RecoverStateRPC)}({state}");
            var participant = participants.FirstOrDefault(part => part.userID == userID);
            if (participant == null) {
                throw new Exception("No participant to recover");
            }
            participant.state = inRound ? (RoundState)state : RoundState.Waiting;
            participant.placedCardID = placedCard;
            participant.baseValue = baseValue;
            participant.bonusValue = bonusValue;
        }

        #region RoundBehaviour
        public void NewRound() {
            Logger.Log($"{name}.{nameof(NewRound)}");
            if (PhotonNetwork.IsMasterClient && !inRound) {
                if (currentRound >= maxRounds) {
                    photonView.RPC(nameof(EndGameRPC), RpcTarget.AllBuffered);
                } else {
                    photonView.RPC(nameof(NewRoundRPC), RpcTarget.AllBuffered);
                }
            }
        }

        [PunRPC]
        void NewRoundRPC() {
            Logger.Log($"{name}.{nameof(NewRoundRPC)}");
            currentRound++;
            inRound = true;
            onNewRound?.Invoke();
            SetDemand();
            StartPlacingRound();
            UpdateRoomProperties();
        }

        void SetDemand() {
            if (PhotonNetwork.IsMasterClient) {
                var newDemand = currentRound <= scriptedRounds.Length
                    ? scriptedRounds[currentRound - 1].randomDemand
                    : defaultRound.randomDemand;
                photonView.RPC("SetDemandRPC", RpcTarget.AllBuffered, (int)newDemand);
            }
        }

        [PunRPC]
        void SetDemandRPC(int demand) {
            Logger.Log($"{name}.{nameof(SetDemandRPC)}({demand}");
            currentDemand = (EQxVariableType)demand;
            onNewDemand?.Invoke(currentDemand);
        }

        void StartPlacingRound() {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC("StartPlacingRoundRPC", RpcTarget.AllBuffered);
            }
        }

        [PunRPC]
        void StartPlacingRoundRPC() {
            Logger.Log($"{name}.{nameof(StartPlacingRoundRPC)}");
            foreach (var player in participants) {
                player.state = RoundState.Start;
                if (player.active) {
                    DetermineNextAction(player);
                }
            }
            onPlacingStarted?.Invoke();
        }

        void TryEndRound() {
            if (participants.Where(cardPlayer => cardPlayer.active).All(cardPlayer => cardPlayer.state == RoundState.Betted)) {
                if (PhotonNetwork.IsMasterClient) {
                    photonView.RPC("EndBettingRoundRPC", RpcTarget.AllBuffered);
                }
            }
        }

        [PunRPC]
        void EndBettingRoundRPC() {
            Logger.Log($"{name}.{nameof(EndBettingRoundRPC)}");
            inRound = false;
            var winner = AllActiveParticipants().Aggregate((x, y) => x.combinedValue > y.combinedValue ? x : y);
            foreach(var player in AllActiveParticipants()) {
                if(player == winner) {
                    player.state = RoundState.Won;
                } else {
                    player.state = RoundState.Lost;
                }
                DetermineNextAction(player);
            }
            onBettingEnded?.Invoke();
        }

        void EndGame() {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC("EndGameRPC", RpcTarget.AllBuffered);
            }
        }

        [PunRPC]
        void EndGameRPC() {
            Logger.Log($"{name}.{nameof(EndGameRPC)}");
            if (!gameOver) {
                gameOver = true;
                currentRound++;
                onGameEnd?.Invoke();
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
        #endregion

        #region Setup

        private void Awake() {
            if (instance) {
                Destroy(gameObject);
                return;
            } else {
                instance = this;
            }
        }

        private void OnDestroy() {
            if (instance == this)
                instance = null;
        }

        void Start() {
            if (PhotonNetwork.IsConnected) {
                maxRounds = (int)PhotonNetwork.CurrentRoom.CustomProperties[TableBrowser.MAX_ROUNDS];
            }
        }

        #endregion
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        }


        private void UpdateRoomProperties() {
            if (PhotonNetwork.IsMasterClient) {
                var props = PhotonNetwork.CurrentRoom.CustomProperties;
                props[TableBrowser.CONNECTED_PLAYERS] = AllActiveParticipants().Select(part => part.player.playerName).ToArray();
                props[TableBrowser.CURRENT_ROUND] = currentRound;
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }
        }

        public IEnumerable<RoundParticipant> AllActiveParticipants() {
            return participants.Where(part => part.active);
        }

        public RoundParticipant GetParticipant(CardPlayer player) {
            return participants.First(part => part.player == player);
        }

        public bool IsInState(CardPlayer player, RoundState state) {
            return GetParticipant(player).state == state;
        }
    }
}
