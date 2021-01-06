using EQx.Game.Player;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace EQx.Game.Investing {
    public class InvestmentManager : MonoBehaviourPunCallbacks, IPunObservable {

        public static InvestmentManager instance = null;

        [SerializeField, Range(0, 100)]
        int initialCapital = 10;
        [SerializeField, Range(0, 10)]
        int blind = 2;
        [SerializeField]
        AnimationCurve investmentPayoff = default;

        public int prize = 0;

        public UnityAction onAllBlindsPayed;

        public List<Account> accounts = new List<Account>();

        List<CardPlayer> payedBlind = new List<CardPlayer>();

        public void Register(CardPlayer player) {
            Debug.Log(name + ".Register: " + player);
            player.onReceivedCoins += ReceivedCoinsListener;
            player.onInvestedCoins += InvestedCoinsListener;
            player.onPayedBlind += PayedBlindListener;
            player.onExtractedCoins += ExtractedCoinsListener;

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
            prize += account.TakeCommitment();
            account.isActive = false;
            player.onReceivedCoins -= ReceivedCoinsListener;
            player.onInvestedCoins -= InvestedCoinsListener;
            player.onPayedBlind -= PayedBlindListener;
            player.onExtractedCoins -= ExtractedCoinsListener;
        }


        private void ExtractedCoinsListener(CardPlayer player, int amount) {
            var account = accounts.Where(acc => acc.player == player).First();
            if (account.CanExtract(amount)) {
                account.extraction += amount;
                account.capital += amount;
            }
        }

        private void PayedBlindListener(CardPlayer player) {
            Debug.Log(name + ".PayedBlind: " + player);
            payedBlind.Add(player);
            var account = accounts.Where(acc => acc.player == player).First();
            if (account.AbleToPay(blind)) {
                account.capital -= blind;
                account.payedBlind += blind;
            } else {
                account.payedBlind = account.capital;
                account.capital -= account.capital;
            }

            if (accounts.Where(acc => acc.isActive).All(acc => payedBlind.Contains(acc.player))){
                onAllBlindsPayed?.Invoke();
                payedBlind.Clear();
            }
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
        }

        private void ReceivedCoinsListener(CardPlayer player, int amount) {
            Debug.Log(name + ".ReceivedCoins: "+ player + ","+ amount);
            var account = accounts.Where(acc => acc.player == player).First();
            account.capital += amount;
        }

        public void WinPrize(List<CardPlayer> winners) {
            prize += accounts.Sum(acc => acc.TakeCommitment());
            int share = prize / winners.Count;
            foreach(var winner in winners) {
                winner.ReceiveCoins(share);
            }
            prize -= share * winners.Count;
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

        public int Investment(CardPlayer player) {
            return accounts.Where(acc => acc.player == player).First().investment;
        }

        public int Commitment(CardPlayer player) {
            return accounts.Where(acc => acc.player == player).First().commitment;
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
