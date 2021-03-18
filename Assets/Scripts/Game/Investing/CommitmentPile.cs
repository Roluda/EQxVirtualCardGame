using EQx.Game.Player;
using TMPro;
using UnityEngine;

namespace EQx.Game.Investing {
    public class CommitmentPile : MonoBehaviour {

        [SerializeField]
        CoinPile creationPile = default;
        [SerializeField]
        CoinPile extractionPile = default;
        [SerializeField]
        TMP_Text infoText = default;
        [SerializeField]
        string infoPrefix = "Commitment: ";

        CardPlayer observedPlayer;
        public int backup = 0;

        public void Initialize(CardPlayer player) {
            Debug.Log(name + "Initialize: " + player);
            observedPlayer = player;
            observedPlayer.onStartedBetting += UpdateCommitment;
            observedPlayer.onEndedBetting += UpdateCommitment;
        }

        public void UpdateCommitment(CardPlayer player) {
            int amount = InvestmentManager.instance.Commitment(player);
            if(amount > 0) {
                extractionPile.SetAmount(0);
                creationPile.SetAmount(amount);
            } else {
                extractionPile.SetAmount(-amount);
                creationPile.SetAmount(0);
            }
            backup = amount;
        }

        public void SetAmountUnsaved(int amount) {
            if (amount > 0) {
                extractionPile.SetAmount(0);
                creationPile.SetAmount(amount);
            } else {
                extractionPile.SetAmount(-amount);
                creationPile.SetAmount(0);
            }
        }

        private void Start() {
            InvestmentManager.instance.onPayedBlind += PayedBlindListener;
            InvestmentManager.instance.onInvested += InvestedCoinsListener;
            InvestmentManager.instance.onCommited += CommitedListener;
        }

        private void Update() {
            infoText.text = $"{infoPrefix}{creationPile.amount - extractionPile.amount}";
            infoText.gameObject.SetActive(creationPile.highlighted || extractionPile.highlighted);
        }

        private void PayedBlindListener(CardPlayer player) {
            Debug.Log(name + "PayedBlindListener: " + player);
            if (player == observedPlayer) {
                int amount = InvestmentManager.instance.PayedBlind(player);
                if (amount > 0) {
                    extractionPile.SetAmount(0);
                    creationPile.SetAmount(amount);
                } else {
                    extractionPile.SetAmount(-amount);
                    creationPile.SetAmount(0);
                }
                backup = amount;
            }
        }

        private void InvestedCoinsListener(CardPlayer player) {
            Debug.Log(name + "InvestedCoinsListener");
            if (player == observedPlayer) {
                int amount = InvestmentManager.instance.Investment(player);
                if (amount > 0) {
                    int excess = extractionPile.AddCoins(-amount);
                    creationPile.AddCoins(-excess);
                } else {
                    int excess = creationPile.AddCoins(amount);
                    extractionPile.AddCoins(-excess);
                }
                backup += amount;
            }
        }

        private void CommitedListener(CardPlayer player) {
            Debug.Log(name + "CommitedListener");
            if (player == observedPlayer) {
                creationPile.SetAmount(0);
                extractionPile.SetAmount(0);
                backup = 0;
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
