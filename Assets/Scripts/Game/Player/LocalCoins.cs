using EQx.Game.Investing;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EQx.Game.Player {
    public class LocalCoins : MonoBehaviour {

        [SerializeField]
        InvestmentInterface laptop = default;
        [SerializeField]
        PileMountain capitalMountain = default;
        [SerializeField]
        PileMountain debtMountain = default;
        [SerializeField]
        CommitmentPile commitmentPile = default;

        int capitalBackup;

        int capital {
            get {
                return capitalMountain.capital - debtMountain.capital;
            }
            set {
                capitalMountain.capital = value;
                debtMountain.capital = -value;
            }
        }

        [SerializeField]
        int debugCapital = 0;
        [SerializeField]
        bool setDebugCapital = false;

        private void OnValidate() {
            if (setDebugCapital) {
                setDebugCapital = false;
                capital = debugCapital;
            }
        }

        // Start is called before the first frame update
        void Start() {
            InvestmentManager.instance.onCapitalUpdated += CapitalUpdatedListener;
            laptop.onInvestmentChange += InvestmentChangedListener;
        }

        private void InvestmentChangedListener(int investment) {
            Debug.Log("backup: " + commitmentPile.backup + " investment: " + investment);
            commitmentPile.SetAmount(commitmentPile.backup + investment);
            capital = capitalBackup - investment;
        }

        private void Awake() {
            CardPlayer.localPlayerReady += commitmentPile.Initialize;
        }

        void CapitalUpdatedListener(CardPlayer player) {
            if (player == CardPlayer.localPlayer) {
                Debug.Log(name + ".CapitalUpdateListener: new Capital: " + InvestmentManager.instance.Capital(player));
                capital = InvestmentManager.instance.Capital(player);
                capitalBackup = capital;
            }
        }

        private void OnDestroy() {
            CardPlayer.localPlayerReady -= commitmentPile.Initialize;
        }

    }
}
