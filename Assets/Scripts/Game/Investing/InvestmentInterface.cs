using EQx.Game.CountryCards;
using EQx.Game.Player;
using EQx.Game.Screen;
using EQx.Game.Table;
using EQx.Game.UI;
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
        [SerializeField]
        BlinkingImage warning = default;
        [SerializeField]
        float timeUntilWarning = 8;

        public UnityAction<int> onInvestmentChange;

        EQxCountryData currentCountry;
        EQxVariableData currentVariable;
        public int plannedInvestment = 0;

        float cardValue => currentCountry.GetValue(currentVariable.type);
        float bonusValue => InvestmentManager.instance.BonusValue(plannedInvestment);

        bool commited = false;
        float timer = 0;

        private void Awake() {
            CardPlayer.localPlayerReady += Initialize;
        }
        void Start() {
            RoundManager.instance.onNewDemand += NewDemandListener;
            confirmButton.onClick.AddListener(ConfirmCommitment);
            investmentSlider.onCommitmentUpdate += AdjustCommitment;
        }
        private void Update() {
            timer += Time.deltaTime;
            if (timer > timeUntilWarning) {
                warning.StartBlink();
            }
        }
        public void AdjustCommitment(int investment) {
            plannedInvestment = investment;
            onInvestmentChange?.Invoke(investment);
            UpdateSliderValues();
        }

        public void ConfirmCommitment() {
            if (!commited) {
                commited = true;
                CardPlayer.localPlayer.InvestCoins(plannedInvestment);
                CardPlayer.localPlayer.Commit();
            }
        }
        private void NewDemandListener(EQxVariableType variable) {
            currentVariable = EQxVariableDatabase.instance.GetVariable(variable);
            UpdateSliderAppearance();
        }
        private void StartedBettingListener(CardPlayer player, int round) {
            int id = RoundManager.instance.GetParticipant(player).placedCardID;
            currentCountry = CountryCardDatabase.instance.GetCountry(id);
            SetHeader();
            UpdateSliderValues();
            commited = false;
            confirmButton.gameObject.SetActive(true);
            warning.StopBlink();
            timer = 0;
            ScreenOn();
        }
        private void EndedBettingListener(CardPlayer player, int round) {
            confirmButton.gameObject.SetActive(false);
        }
        private void ScreenOn() {
            screen.gameObject.SetActive(true);
            investmentSlider.Reset();
            warning.StopBlink();
        }

        public void Initialize(CardPlayer player) {
            CardPlayer.localPlayerReady -= Initialize;
            player.onStartedBetting += StartedBettingListener;
            player.onEndedBetting += EndedBettingListener;
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
            } else if (bonusValue < 0) {
                addedValue.SetValueInstant(cardValue);
                actualValue.SetValue(cardValue + bonusValue);
                reducedValue.SetValueInstant(cardValue);
            } else {
                addedValue.SetValue(cardValue);
                actualValue.SetValue(cardValue);
                reducedValue.SetValue(cardValue);
            }
            combinedValue.text = ((int)(cardValue + bonusValue)).ToString();
        }
    }
}
