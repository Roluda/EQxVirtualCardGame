using EQx.Game.CountryCards;
using EQx.Game.Investing;
using EQx.Game.Player;
using EQx.Game.Table;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EQx.Game.Screen {
    public class WinnerEntry : MonoBehaviour {

        [SerializeField]
        Image flagIcon = default;
        [SerializeField]
        string flagPath = "Flags";
        [SerializeField]
        TMP_Text label = default;
        [SerializeField]
        string nameConnector = " - ";
        [SerializeField]
        TMP_Text value = default;

        [SerializeField]
        ValueSlider actualValue = default;
        [SerializeField]
        ValueSlider addedValue = default;
        [SerializeField]
        ValueSlider reducedValue = default;

        [SerializeField]
        Color loseColor = default;
        [SerializeField]
        float winHeight = 300;

        public float presentSpeed = 5;

        EQxVariableData demand => EQxVariableDatabase.instance.GetVariable(RoundManager.instance.currentDemand);
        public CardPlayer player { get; private set; }
        EQxCountryData playedCard;

        bool winner = false;

        float currentValue = 0;
        float targetValue = 0;
        float startValue;
        bool presentingValues = false;

        public void Initialize(CardPlayer player, float startValue, float targetValue, float presentTime) {
            this.player = player;
            currentValue = startValue;
            var participant = RoundManager.instance.GetParticipant(player);
            this.startValue = startValue;
            this.targetValue = participant.combinedValue;
            presentSpeed = (targetValue - startValue) / presentTime;
            playedCard = CountryCardDatabase.instance.GetCountry(participant.placedCardID);
            winner = participant.state == RoundState.Won ? true : false;
            label.text = player.playerName + nameConnector + playedCard.countryName;
            var sprite = Resources.Load<Sprite>(flagPath + "/" + playedCard.isoCountryCode.ToLower());
            if (sprite == null) {
                sprite = Resources.Load<Sprite>(flagPath + "/un");
            }
            flagIcon.sprite = sprite;
            SetSliderAppearance();
            UpdateSliderValuesInstant();
        }

        public void PresentValue() {
            presentingValues = true;
        }

        public void Win() {
            if (winner) {
                if (TryGetComponent<RectTransform>(out var rect)) {
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, winHeight);
                }
            } else {
                Lose();
            }
        }

        public void Lose() {
            reducedValue.color = loseColor;
            addedValue.color = loseColor;
            actualValue.color = loseColor;
            label.color = loseColor;
            flagIcon.color = loseColor;
            value.color = loseColor;
        }

        public void Clear() {
            Destroy(gameObject);
        }

        void Update() {
            if (presentingValues && !ReachedTarget()) {
                currentValue += Time.deltaTime * presentSpeed;
                UpdateSliderValuesInstant();
            }
        }

        bool ReachedTarget() {
            return startValue > targetValue
                ? currentValue <= targetValue
                : currentValue >= targetValue;
        }

        void SetSliderAppearance() {
            actualValue.color = demand.color;
            addedValue.color = demand.color;
            reducedValue.color = demand.color;
        }

        void UpdateSliderValuesInstant() {
            actualValue.SetValueInstant(currentValue);
            addedValue.SetValueInstant(currentValue);
            reducedValue.SetValueInstant(currentValue);
        }
    }
}
