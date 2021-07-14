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
        public int prize {
            get => prizePool;
            set {
                int oldPool = prizePool;
                prizePool = value;
                if (oldPool != prizePool) {
                    onPrizeUpdated?.Invoke();
                }
            }
        }

        public UnityAction onPrizeUpdated;
        public UnityAction<int> onEconomyGrowth;
        public UnityAction<CardPlayer, int> onWinPrize;
        public UnityAction<CardPlayer> onCapitalUpdated;
        public UnityAction<CardPlayer> onReceivedCoins;
        public UnityAction<CardPlayer> onPayedBlind;
        public UnityAction<CardPlayer> onInvested;
        public UnityAction<CardPlayer> onCommited;

        public List<Account> accounts = new List<Account>();

        public CardPlayer prizeWinner;

        public void Register(CardPlayer player) {
            player.onReceivedCoins += ReceivedCoinsListener;
            player.onInvestedCoins += InvestedCoinsListener;
            player.onPayedBlind += PayedBlindListener;
            player.onCommited += CommitedListener;
            player.onWin += WinListener;

            string userID = player.photonView.Owner.UserId;
            var account = accounts.Where(acc => acc.userID == userID).FirstOrDefault();
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
            account.isActive = false;
            player.onReceivedCoins -= ReceivedCoinsListener;
            player.onInvestedCoins -= InvestedCoinsListener;
            player.onPayedBlind -= PayedBlindListener;
            player.onCommited -= CommitedListener;
            player.onWin -= WinListener;
        }

        private void PayedBlindListener(CardPlayer player) {
            var account = accounts.Where(acc => acc.player == player).First();
            account.capital -= blind;
            account.payedBlind += blind;
            onPayedBlind?.Invoke(player);
            onCapitalUpdated?.Invoke(player);
        }

        private void InvestedCoinsListener(CardPlayer player, int amount) {
            var account = accounts.Where(acc => acc.player == player).First();
            account.capital -= amount;
            account.investment += amount;
            onInvested?.Invoke(player);
            onCapitalUpdated?.Invoke(player);
        }

        private void ReceivedCoinsListener(CardPlayer player, int amount) {
            var account = accounts.Where(acc => acc.player == player).First();
            account.capital += amount;
            onReceivedCoins?.Invoke(player);
            onCapitalUpdated?.Invoke(player);
        }

        private void CommitedListener(CardPlayer player) {
            AddPrize(TakeCommitment(player));
            onCommited?.Invoke(player);
        }

        private void WinListener(CardPlayer player) {
            prizeWinner = player;
        }

        public void CommitAll() {
            foreach(var account in accounts.Where(acc => acc.isActive)) {
                account.player.Commit();
            }
        }

        [PunRPC]
        void SetPrizeRPC(int amount) {
            Logger.Log($"{name}.{nameof(SetPrizeRPC)}");
            prize = amount;
        }

        [PunRPC]
        void AddPrizeRPC(int amount) {
            Logger.Log($"{name}.{nameof(AddPrizeRPC)}({amount})");
            prize += amount;
        }

        [PunRPC]
        void EconomyGrowthRPC() {
            Logger.Log($"{name}.{nameof(EconomyGrowthRPC)}");
            float currentPrize = prize;
            int newPrize = (int)(currentPrize * economicGrowth);
            onEconomyGrowth?.Invoke(newPrize - prize);
            prize = newPrize;
        }

        [PunRPC]
        void WinPrizeRPC() {
            Logger.Log($"{name}.{nameof(WinPrizeRPC)}");
            if (prizeWinner) {
                prizeWinner.ReceiveCoins(prize);
                onWinPrize?.Invoke(prizeWinner, prize);
                SetPrize(0);
            }
        }

        public void SetPrize(int amount) {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC(nameof(SetPrizeRPC), RpcTarget.AllBuffered, amount);
            }
        }

        public void AddPrize(int amount) {
            if (PhotonNetwork.IsMasterClient) {
                photonView.RPC(nameof(AddPrizeRPC), RpcTarget.AllBuffered, amount);
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
