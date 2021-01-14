﻿using EQx.Game.CountryCards;
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

        float bonusValue = 0;
        float baseValue = 0;
        bool winner = false;

        public float combinedValue => baseValue + bonusValue;

        float currentValue = 0;
        bool presentingBaseValue = false;

        public void Initialize(PlayerStats stats) {
            player = stats.player;
            playedCard = CountryCardDatabase.instance.GetCountry(stats.placedCard);
            bonusValue = stats.bonusValue;
            baseValue = stats.baseValue;
            winner = stats.won;
            label.text = player.playerName + nameConnector + playedCard.countryName;
            flagIcon.sprite = Resources.Load<Sprite>(flagPath + "/" + this.playedCard.isoCountryCode.ToLower());
            SetSliderAppearance();
            UpdateSliderValuesInstant();
        }

        public void PresentBaseValues() {
            presentingBaseValue = true;
        }

        public void PresentBonusValues() {
            presentingBaseValue = false;
            if (bonusValue > 0) {
                addedValue.SetValue(baseValue + bonusValue);
            } else {
                reducedValue.SetValue(baseValue + bonusValue);
            }
        }

        public void ApplyBonusValues() {
            addedValue.SetValue(baseValue + bonusValue);
            actualValue.SetValue(baseValue + bonusValue);
            reducedValue.SetValue(baseValue + bonusValue);
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


        // Update is called once per frame
        void Update() {
            if (presentingBaseValue) {
                if (currentValue < baseValue) {
                    currentValue += Time.deltaTime * presentSpeed;
                }
                UpdateSliderValuesInstant();
            }
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



        // Start is called before the first frame update
        void Start() {

        }

    }
}