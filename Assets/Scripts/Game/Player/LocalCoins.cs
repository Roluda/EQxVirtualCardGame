using EQx.Game.Investing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Player {
    public class LocalCoins : MonoBehaviour {

        [SerializeField]
        InvestmentInterface laptop = default;
        [SerializeField]
        PileMountain capitalMountain = default;
        [SerializeField]
        CommitmentPile commitmentPile = default;

        int capitalBackup;

        // Start is called before the first frame update
        void Start() {
            InvestmentManager.instance.onCapitalUpdated += CapitalUpdatedListener;
            laptop.onAdjustedCommit += CommitAdjustmListener;
        }

        private void CommitAdjustmListener() {
            Debug.Log("backup: " + commitmentPile.backup + " investment: " + laptop.plannedInvestment + " extraction: "+laptop.plannedExtraction);
            commitmentPile.targetAmount = commitmentPile.backup + laptop.plannedInvestment - laptop.plannedExtraction;
            capitalMountain.capital = capitalBackup - laptop.plannedInvestment + laptop.plannedExtraction;
        }

        private void Awake() {
            CardPlayer.localPlayerReady += commitmentPile.Initialize;
        }

        void CapitalUpdatedListener(CardPlayer player) {
            if (player == CardPlayer.localPlayer) {
                Debug.Log(name + ".CapitalUpdateListener: new Capital: " + InvestmentManager.instance.Capital(player));
                capitalMountain.capital = InvestmentManager.instance.Capital(player);
                capitalBackup = capitalMountain.capital;
            }
        }

        private void OnDestroy() {
            CardPlayer.localPlayerReady -= commitmentPile.Initialize;
        }

    }
}
