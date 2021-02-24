using EQx.Game.Player;
using UnityEngine;

namespace EQx.Game.Investing {
    public class CommitmentPile : CoinPile {

        CardPlayer observedPlayer;
        public int backup = 0;

        public void Initialize(CardPlayer player) {
            Debug.Log(name + "Initialize: " + player);
            observedPlayer = player;
            observedPlayer.onStartedBetting += UpdateCommitment;
            observedPlayer.onEndedBetting += UpdateCommitment;
        }

        public void UpdateCommitment(CardPlayer player) {
            SetAmount(InvestmentManager.instance.Commitment(player));
            backup = amount;
        }

        private void Start() {
            InvestmentManager.instance.onPayedBlind += PayedBlindListener;
            InvestmentManager.instance.onInvested += InvestedCoinsListener;
            InvestmentManager.instance.onCommited += CommitedListener;
        }

        private void PayedBlindListener(CardPlayer player) {
            Debug.Log(name + "PayedBlindListener: " + player);
            if (player == observedPlayer) {
                AddCoins(InvestmentManager.instance.PayedBlind(player));
                backup = amount;
            }
        }

        private void InvestedCoinsListener(CardPlayer player) {
            Debug.Log(name + "InvestedCoinsListener");
            if (player == observedPlayer) {
                AddCoins(InvestmentManager.instance.Investment(player));
                backup = amount;
            }
        }

        private void CommitedListener(CardPlayer player) {
            Debug.Log(name + "CommitedListener");
            if (player == observedPlayer) {
                SetAmount(0);
                backup = amount;
            }
        }

        private void OnDestroy() {
            if (InvestmentManager.instance != null) {
                InvestmentManager.instance.onPayedBlind -= PayedBlindListener;
                InvestmentManager.instance.onInvested -= InvestedCoinsListener;
                InvestmentManager.instance.onCommited -= CommitedListener;
            }
        }
    }
}
