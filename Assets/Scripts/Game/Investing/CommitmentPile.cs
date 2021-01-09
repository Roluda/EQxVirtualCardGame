﻿using EQx.Game.Player;
using UnityEngine;

namespace EQx.Game.Investing {
    public class CommitmentPile : CoinPile {

        CardPlayer observedPlayer;
        public int backup = 0;

        public void Initialize(CardPlayer player) {
            Debug.Log(name + "Initialize: " + player);
            observedPlayer = player;
            observedPlayer.onStartedTurn += UpdateCommitment;
            observedPlayer.onEndedTurn += UpdateCommitment;
        }

        public void UpdateCommitment(CardPlayer player) {
            targetAmount = InvestmentManager.instance.Commitment(player);
            backup = targetAmount;
        }

        private void Start() {
            InvestmentManager.instance.onPayedBlind += PayedBlindListener;
            InvestmentManager.instance.onExtracted += ExtractedCoinsListener;
            InvestmentManager.instance.onInvested += InvestedCoinsListener;
            InvestmentManager.instance.onCommited += CommitedListener;
        }

        private void PayedBlindListener(CardPlayer player) {
            Debug.Log(name + "PayedBlindListener: " + player);
            if (player == observedPlayer) {
                AddCoins(InvestmentManager.instance.PayedBlind(player));
                backup = targetAmount;
            }
        }

        private void InvestedCoinsListener(CardPlayer player) {
            Debug.Log(name + "InvestedCoinsListener");
            if (player == observedPlayer) {
                AddCoins(InvestmentManager.instance.Investment(player));
                backup = targetAmount;
            }
        }

        private void ExtractedCoinsListener(CardPlayer player) {
            Debug.Log(name + "ExtractedCoinsListener");
            if (player == observedPlayer) {
                RemoveCoins(InvestmentManager.instance.Extraction(player));
                backup = targetAmount;
            }
        }

        private void CommitedListener(CardPlayer player) {
            Debug.Log(name + "CommitedListener");
            if (player == observedPlayer) {
                RemoveCoins(targetAmount);
                backup = targetAmount;
            }
        }

        private void OnDestroy() {
            InvestmentManager.instance.onPayedBlind -= PayedBlindListener;
            InvestmentManager.instance.onExtracted -= ExtractedCoinsListener;
            InvestmentManager.instance.onInvested -= InvestedCoinsListener;
            InvestmentManager.instance.onCommited -= CommitedListener;
        }
    }
}
