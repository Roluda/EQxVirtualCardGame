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
        int blind = 2;
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
        public UnityAction<CardPlayer> onCapitalUpdated;
        public UnityAction<CardPlayer> onPayedBlind;
        public UnityAction<CardPlayer> onInvested;
        public UnityAction<CardPlayer> onExtracted;
        public UnityAction<CardPlayer> onCommited;

        public List<Account> accounts = new List<Account>();

        public void Register(CardPlayer player) {
            Debug.Log(name + ".Register: " + player);
            player.onReceivedCoins += ReceivedCoinsListener;
            player.onInvestedCoins += InvestedCoinsListener;
            player.onPayedBlind += PayedBlindListener;
            player.onExtractedCoins += ExtractedCoinsListener;
            player.onCommited += CommitedListener;

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
            player.Commit();
            account.isActive = false;
            player.onReceivedCoins -= ReceivedCoinsListener;
            player.onInvestedCoins -= InvestedCoinsListener;
            player.onPayedBlind -= PayedBlindListener;
            player.onExtractedCoins -= ExtractedCoinsListener;
            player.onCommited -= CommitedListener;
        }


        private void ExtractedCoinsListener(CardPlayer player, int amount) {
            Debug.Log(name + ".ExtractedCoins: " + player + "," + amount);
            var account = accounts.Where(acc => acc.player == player).First();
            if (account.CanExtract(amount)) {
                account.extraction += amount;
                account.capital += amount;
            }
            onExtracted?.Invoke(player);
            onCapitalUpdated?.Invoke(player);
        }

        private void PayedBlindListener(CardPlayer player) {
            Debug.Log(name + ".PayedBlind: " + player);
            var account = accounts.Where(acc => acc.player == player).First();
            if (account.AbleToPay(blind)) {
                account.capital -= blind;
                account.payedBlind += blind;
            } else {
                account.payedBlind = account.capital;
                account.capital -= account.capital;
            }
            Debug.Log(player + " payed " + account.payedBlind);
            onPayedBlind?.Invoke(player);
            onCapitalUpdated?.Invoke(player);
        }

        private void InvestedCoinsListener(CardPlayer player, int amount) {
            Debug.Log(name + ".InvestedCoins: " + player + "," + amount);
            var account = accounts.Where(acc => acc.player == player).First();
            if (account.AbleToPay(amount)) {
                account.capital -= amount;
                account.investment += amount;
            } else {
                account.investment += account.capital;
                account.capital -= account.capital;
            }
            onInvested?.Invoke(player);
            onCapitalUpdated?.Invoke(player);
        }

        private void ReceivedCoinsListener(CardPlayer player, int amount) {
            Debug.Log(name + ".ReceivedCoins: "+ player + ","+ amount);
            var account = accounts.Where(acc => acc.player == player).First();
            account.capital += amount;
            onCapitalUpdated?.Invoke(player);
        }

        private void CommitedListener(CardPlayer player) {
            Debug.Log(name + ".CommitedListener: "+ player);
            prize += TakeCommitment(player);
            onCommited?.Invoke(player);
        }

        public void CommitAll() {
            foreach(var account in accounts.Where(acc => acc.isActive)) {
                account.player.Commit();
            }
        }

        public void WinPrize() {
            var winners = RoundManager.instance.winners;
            if (winners.Count > 0) {
                int share = prize / winners.Count;
                foreach (var winner in winners) {
                    winner.ReceiveCoins(share);
                }
                prize -= share * winners.Count;
            }
        }


        public bool AbleToPay(CardPlayer player, int amount) {
            return accounts.Where(acc => acc.player == player).First().AbleToPay(amount);
        }

        public bool CanExtract(CardPlayer player, int amount) {
            return accounts.Where(acc => acc.player == player).First().CanExtract(amount);
        }

        public bool SufficientBlind(CardPlayer player) {
            return accounts.Where(acc => acc.player == player).First().SufficientBlind(blind);
        }

        public int Capital(CardPlayer player) {
            return accounts.Where(acc => acc.player == player).First().capital;
        }

        public int Investment(CardPlayer player) {
            return accounts.Where(acc => acc.player == player).First().investment;
        }

        public int Extraction(CardPlayer player) {
            return accounts.Where(acc => acc.player == player).First().extraction;
        }

        public int PayedBlind(CardPlayer player) {
            return accounts.Where(acc => acc.player == player).First().payedBlind;
        }

        public int Commitment(CardPlayer player) {
            return accounts.Where(acc => acc.player == player).First().commitment;
        }

        public int TakeCommitment(CardPlayer player) {
            return accounts.Where(acc => acc.player == player).First().TakeCommitment();
        }

        public float BonusValue(int investment, int extraction) {
            return investmentPayoff.Evaluate(investment-extraction);
        }

        public float BonusValue(CardPlayer player) {
            return investmentPayoff.Evaluate(Investment(player)-Extraction(player));
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
