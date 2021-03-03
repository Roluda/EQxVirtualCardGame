using EQx.Game.Investing;
using EQx.Game.Player;
using EQx.Game.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EQx.Game.Statistics {
    public class PlayerObserver : MonoBehaviour {

        public static PlayerObserver instance;

        List<PlayerTrack> playerTracks = new List<PlayerTrack>();

        int currentRound = 0;

        public void Register(CardPlayer player) {
            Debug.Log(name + ".Register: " + player);
            player.onInvestedCoins += InvestedCoinsListener;
            player.onLost += LoseListener;
            player.onWin += WinListener;
            player.onPlacedCard += CardPlacedListener;
            player.onStartedPlacing += StartPlacingListener;

            string userID = player.photonView.Owner.UserId;
            var playerTrack = playerTracks.Where(track => track.userID == userID).FirstOrDefault();
            if (playerTrack == null) {
                playerTracks.Add(new PlayerTrack(player));
            } else {
                playerTrack.player = player;
            }
        }

        public void Unregister(CardPlayer player) {
            player.onInvestedCoins -= InvestedCoinsListener;
            player.onLost -= LoseListener;
            player.onWin -= WinListener;
            player.onPlacedCard -= CardPlacedListener;
            player.onStartedPlacing -= StartPlacingListener;
        }

        private void WinListener(CardPlayer player) {
            AddWin(player, true);
            AddWinnings(player, InvestmentManager.instance.prize - InvestmentManager.instance.LastCommitment(player));
        }

        private void InvestedCoinsListener(CardPlayer player, int amount) {
            AddInvestment(player, amount);
            var investments = GetTrack(player).investments.Values.ToList();
            float extraction = Mathf.Abs(investments.Where(inv => inv <= 0).Count());
            float creation = investments.Where(inv => inv > 0).Count();
            float vcp = creation / (extraction + creation);
            AddVCP(player, vcp);
        }

        private void CommitedListener(CardPlayer player) {
            AddCommitment(player, InvestmentManager.instance.LastCommitment(player));
        }

        private void CardPlacedListener(CardPlayer player, int id) {
            AddCardPlayed(player, id);
        }

        private void LoseListener(CardPlayer player) {
            AddWin(player, false);
            AddWinnings(player, -InvestmentManager.instance.LastCommitment(player));
        }

        private void StartPlacingListener(CardPlayer player) {
            AddCapital(player, InvestmentManager.instance.Capital(player));
        }

        #region Data API
        public int GetWinnings(CardPlayer player) {
            return GetWinnings(player, currentRound);
        }

        public int GetWinnings(CardPlayer player, int round) {
            return GetTrack(player).winnings[round];
        }

        public bool GetWin(CardPlayer player) {
            return GetWin(player, currentRound);
        }

        public bool GetWin(CardPlayer player, int round) {
            return GetTrack(player).won[round];
        }

        public int GetCapital(CardPlayer player) {
            return GetCapital(player, currentRound);
        }

        public int GetCapital(CardPlayer player, int round) {
            return GetTrack(player).capital[round];
        }

        public int GetCommitment(CardPlayer player) {
            return GetCommitment(player, currentRound);
        }

        public int GetCommitment(CardPlayer player, int round) {
            return GetTrack(player).commitments[round];
        }

        public int GetInvestment(CardPlayer player) {
            return GetInvestment(player, currentRound);
        }

        public int GetInvestment(CardPlayer player, int round) {
            return GetTrack(player).investments[round];
        }

        public int GetCardPlaced(CardPlayer player) {
            return GetCardPlaced(player, currentRound);
        }

        public int GetCardPlaced(CardPlayer player, int round) {
            return GetTrack(player).cardPlaced[round];
        }

        public float GetVCP(CardPlayer player) {
            return GetVCP(player, currentRound);
        }

        public float GetVCP(CardPlayer player, int round) {
            return GetTrack(player).valueCreationPercentile[round];
        }

        public PlayerTrack GetTrack(CardPlayer player) {
            return playerTracks.Where(track => track.player == player).FirstOrDefault();
        }

        void AddWin(CardPlayer player, bool data) {
            GetTrack(player).won[currentRound] = data;
        }

        void AddWinnings(CardPlayer player, int data) {
            GetTrack(player).winnings[currentRound] = data;
        }

        void AddCapital(CardPlayer player, int data) {
            GetTrack(player).capital[currentRound] = data;
        }

        void AddInvestment(CardPlayer player, int data) {
            GetTrack(player).investments[currentRound] = data;
        }

        void AddCommitment(CardPlayer player, int data) {
            GetTrack(player).commitments[currentRound] = data;
        }

        void AddCardPlayed(CardPlayer player, int data) {
            GetTrack(player).cardPlaced[currentRound] = data;
        }

        void AddVCP(CardPlayer player, float data) {
            GetTrack(player).valueCreationPercentile[currentRound] = data;
        }

        #endregion



        // Start is called before the first frame update
        void Awake() {
            if (instance != null) {
                Destroy(gameObject);
            } else {
                instance = this;
            }
        }

        void Start() {
            RoundManager.instance.onNewRound += NewRoundListener;
            RoundManager.instance.onGameEnd += GameOverListener;
            InvestmentManager.instance.onCommited += CommitedListener;
        }

        private void GameOverListener() {
            Debug.Log(name + ".GameOverListener");
            currentRound = RoundManager.instance.currentRound;
            foreach(var player in RoundManager.instance.registeredPlayers) {
                AddCapital(player, InvestmentManager.instance.Capital(player));
            }

        }

        private void NewRoundListener() {
            currentRound = RoundManager.instance.currentRound;
        }

        private void OnDestroy() {
            if(instance == this) {
                instance = null;
            }
        }
    }
}
