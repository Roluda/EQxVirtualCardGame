using EQx.Game.Player;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using EQx.Game.Table;

namespace EQx.Game.Investing {
    public class InvestmentManager : MonoBehaviourPunCallbacks, IPunObservable {

        public static InvestmentManager instance = null;

        [SerializeField, Range(0, 100)]
        int initialCapital = 10;
        [SerializeField, Range(0, 10)]
        public int blind = 2;
        [SerializeField, Range(1,2)]
        public float economicGrowth = 1.1f;
        [SerializeField]
        public int maxCreation = 3;
        [SerializeField]
        public int maxExtraction = 3;
        [SerializeField]
        AnimationCurve investmentPayoff = default;

        [SerializeField]
        int prizePool = 0;
        public int jackpot {
            get => prizePool;
            set {
                int oldPool = prizePool;
                prizePool = value;
                if (oldPool != prizePool) {
                    onJackpotUpdated?.Invoke();
                }
            }
        }

        public UnityAction onJackpotUpdated;
        public UnityAction<int> onEconomyGrowth;
        public UnityAction<CardPlayer, int> onWinPrize;
        public UnityAction<CardPlayer> onCapitalUpdated;
        public UnityAction<CardPlayer> onReceivedCoins;
        public UnityAction<CardPlayer> onPayedBlind;
        public UnityAction<CardPlayer> onInvested;
        public UnityAction<CardPlayer> onCommited;

        public List<Account> accounts = new List<Account>();

        public CardPlayer jackpotWinner;

        public void Register(CardPlayer player) {
            player.onReceivedCoins += ReceivedCoinsListener;
            player.onInvestedCoins += InvestedCoinsListener;
            player.onPayedBlind += PayedBlindListener;
            player.onCommited += CommitedListener;
            player.onWin += WinListener;

            string userID = player.photonView.Owner.UserId;
            var account = accounts.FirstOrDefault(acc => acc.userID == userID);
            if (account != null) {
                accounts.Add(new Account(player));
                accounts.Remove(account);
                player.ReceiveCoins(account.capital);
            } else {
                accounts.Add(new Account(player));
                player.ReceiveCoins(initialCapital);
            }
        }

        public void Unregister(CardPlayer player) {
            var account = accounts.Where(acc => acc.player == player).First();
            account.capital += account.TakeCommitment();
            if(player == jackpotWinner) {
                account.capital += jackpot;
                SetJackpot(0);
                jackpotWinner = null;
            }
            account.isActive = false;
            player.onReceivedCoins -= ReceivedCoinsListener;
            player.onInvestedCoins -= InvestedCoinsListener;
            player.onPayedBlind -= PayedBlindListener;
            player.onCommited -= CommitedListener;
            player.onWin -= WinListener;
        }

        private void PayedBlindListener(CardPlayer player) {
            var account = accounts.First(acc => acc.player == player);
            account.capital -= blind;
            account.payedBlind += blind;
            onPayedBlind?.Invoke(player);
            onCapitalUpdated?.Invoke(player);
        }

        private void InvestedCoinsListener(CardPlayer player, int amount) {
            var account = accounts.First(acc => acc.player == player);
            account.capital -= amount;
            account.investment += amount;
            onInvested?.Invoke(player);
            onCapitalUpdated?.Invoke(player);
        }

        private void ReceivedCoinsListener(CardPlayer player, int amount) {
            var account = accounts.First(acc => acc.player == player);
            account.capital += amount;
            onReceivedCoins?.Invoke(player);
            onCapitalUpdated?.Invoke(player);
        }

        private void CommitedListener(CardPlayer player) {
            SetJackpot(jackpot+TakeCommitment(player));
            onCommited?.Invoke(player);
        }

        private void WinListener(CardPlayer player) {
            jackpotWinner = player;
        }

        public void CommitAll() {
            foreach(var account in accounts.Where(acc => acc.isActive)) {
                account.player.Commit();
            }
        }

        [PunRPC]
        void SetJackpotRPC(int amount) {
            Logger.Log($"{name}.{nameof(SetJackpotRPC)}");
            jackpot = amount;
        }

        [PunRPC]
        void EconomyGrowthRPC() {
            Logger.Log($"{name}.{nameof(EconomyGrowthRPC)}");
            float currentPrize = jackpot;
            int newPrize = (int)(currentPrize * economicGrowth);
            onEconomyGrowth?.Invoke(newPrize - jackpot);
            jackpot = newPrize;
        }

        [PunRPC]
        void WinPrizeRPC() {
            Logger.Log($"{name}.{nameof(WinPrizeRPC)}");
            if (jackpotWinner) {
                jackpotWinner.ReceiveCoins(jackpot);
                onWinPrize?.Invoke(jackpotWinner, jackpot);
                jackpotWinner = null;
                SetJackpot(0);
            }
        }

        public void SetJackpot(int amount) {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC(nameof(SetJackpotRPC), RpcTarget.AllBuffered, amount);
            }
        }

        public void EconomyGrowth() {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC(nameof(EconomyGrowthRPC), RpcTarget.AllBuffered);
            }
        }

        public void WinPrize() {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC(nameof(WinPrizeRPC), RpcTarget.AllBuffered);
            }
        }

        public int Capital(CardPlayer player) {
            return accounts.Where(acc => acc.player == player).First().capital;
        }

        public int Investment(CardPlayer player) {
            return accounts.Where(acc => acc.player == player).First().investment;
        }

        public int PayedBlind(CardPlayer player) {
            return accounts.Where(acc => acc.player == player).First().payedBlind;
        }

        public int Commitment(CardPlayer player) {
            return accounts.Where(acc => acc.player == player).First().commitment;
        }

        public int LastCommitment(CardPlayer player) {
            return accounts.Where(acc => acc.player == player).First().lastCommitment;
        }

        public int TakeCommitment(CardPlayer player) {
            return accounts.Where(acc => acc.player == player).First().TakeCommitment();
        }

        public float BonusValue(int investment) {
            return investmentPayoff.Evaluate(investment);
        }

        public float BonusValue(CardPlayer player) {
            return investmentPayoff.Evaluate(Investment(player));
        }

        private void Awake() {
            if (instance != null) {
                Destroy(gameObject);
            } else {
                instance = this;
            }
        }

        private void OnDestroy() {
            if (instance == this)
                instance = null;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) { }
    }
}
