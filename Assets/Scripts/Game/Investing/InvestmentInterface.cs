using EQx.Game.CountryCards;
using EQx.Game.Player;
using EQx.Game.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EQx.Game.Investing {
    public class InvestmentInterface : MonoBehaviour {
        [SerializeField]
        GameObject screen = default;

        [Header("Head")]
        [SerializeField]
        Image countryFlag = default;
        [SerializeField]
        TMP_Text countryName = default;
        [SerializeField]
        string flagPath = "Flags";

        [Header("ValueInformation")]
        [SerializeField]
        ValueSlider addedValue = default;
        [SerializeField]
        ValueSlider actualValue = default;
        [SerializeField]
        ValueSlider reducedValue = default;
        [SerializeField]
        Image demandIcon = default;
        [SerializeField]
        TMP_Text combinedValue = default;

        [Header("InvestingInformation")]
        [SerializeField]
        InvestmentSlider investmentSlider = default;

        [Header("Confirmation")]
        [SerializeField]
        Button confirmButton = default;

        public UnityAction<int> onInvestmentChange;

        EQxCountryData currentCountry;
        EQxVariableData currentVariable;
        public int plannedInvestment = 0;

        float cardValue => currentCountry.GetValue(currentVariable.type);
        float bonusValue => InvestmentManager.instance.BonusValue(plannedInvestment);

        bool commited = false;
        // Start is called before the first frame update


        private void NewDemandListener(EQxVariableType variable) {
            currentVariable = EQxVariableDatabase.instance.GetVariable(variable);
            UpdateSliderAppearance();
        }

        void CardPlacedListener(CardPlayer player, int id) {
            ScreenOn();
            currentCountry = CountryCardDatabase.instance.GetCountry(id);
            SetHeader();
            UpdateSliderValues();
        }

        void SetHeader() {
            countryFlag.sprite = Resources.Load<Sprite>(flagPath + "/" + currentCountry.isoCountryCode.ToLower());
            countryName.text = currentCountry.countryName;
        }

        void UpdateSliderAppearance() {
            demandIcon.sprite = currentVariable.iconTransparent;
            actualValue.color = currentVariable.color;
            addedValue.color = currentVariable.color;
            reducedValue.color = currentVariable.color;
            combinedValue.color = currentVariable.color;
        }

        private void UpdateSliderValues() {
            if (bonusValue > 0) {
                addedValue.SetValue(cardValue + bonusValue);
                actualValue.SetValueInstant(cardValue);
                reducedValue.SetValueInstant(0);
            } else {
                addedValue.SetValueInstant(0);
                actualValue.SetValue(cardValue + bonusValue);
                reducedValue.SetValueInstant(cardValue);
            }
            combinedValue.text = ((int)(cardValue + bonusValue)).ToString();
        }


        public void AdjustCommitment(int investment) {
            Debug.Log(name + ".AdjustCommitment: " + investment);
            plannedInvestment = investment;
            onInvestmentChange?.Invoke(investment);
            UpdateSliderValues();
        }

        public void ConfirmCommitment() {
            Debug.Log(name + ".ConfirmCommitment");
            if (!commited) {
                commited = true;
                CardPlayer.localPlayer.InvestCoins(plannedInvestment);
                CardPlayer.localPlayer.EndTurn();
            }
        }

        private void StartedTurnListener(CardPlayer player) {
            Debug.Log(name + ".StartedTurnListener: " + player);
            commited = false;
            confirmButton.gameObject.SetActive(true);
        }

        private void EndedTurnListener(CardPlayer player) {
            Debug.Log(name + ".EndedTurnListener: "+player);
            confirmButton.gameObject.SetActive(false);
        }

        private void ScreenOn() {
            Debug.Log(name + ".ScreenOn");
            screen.gameObject.SetActive(true);
            investmentSlider.Reset();
        }

        private void ScreenOff() {
            Debug.Log(name + ".ScreenOff");
            screen.gameObject.SetActive(false);
        }

        public void Initialize(CardPlayer player) {
            Debug.Log(name + "Initialize");
            CardPlayer.localPlayerReady -= Initialize;
            player.onPlacedCard += CardPlacedListener;
            player.onStartedTurn += StartedTurnListener;
            player.onEndedTurn += EndedTurnListener;
        }

        private void Awake() {
            CardPlayer.localPlayerReady += Initialize;
        }

        void Start() {
            RoundManager.instance.onBettingStarted += ScreenOn;
            RoundManager.instance.onBettingEnded += ScreenOff;
            RoundManager.instance.onNewDemand += NewDemandListener;
            confirmButton.onClick.AddListener(ConfirmCommitment);
            investmentSlider.onCommitmentUpdate += AdjustCommitment;
            ScreenOff();
        }

    }
}
