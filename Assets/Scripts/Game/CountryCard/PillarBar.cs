using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace EQx.Game.CountryCards {
    public class PillarBar : CountryCardComponent {

        [SerializeField]
        public EQxVariableData data;


        [SerializeField]
        Image icon = default;
        [SerializeField]
        Image bar = default;
        [SerializeField]
        TMP_Text value = default;
        [SerializeField]
        TMP_Text label = default;

        [SerializeField]
        float gainRate = 20;

        [SerializeField]
        float maximumValue = 100;

        [SerializeField]
        float maximumBarWidth = 300;
        [SerializeField]
        float valueOffset = 100;
        [SerializeField]
        float barOffset = 50;

        public float targetValue = 70;
        public float currentValue = 50;


        protected override void Validate() {
            InitializePillar();
        }

        // Start is called before the first frame update
        void Start() {

            if (!observedCard) {
                Debug.Log("NoCard");
            }
            if (observedCard.data == null) {
                Debug.Log("Card has no data");
            }
            targetValue = observedCard.data.GetValue(data.variable);
        }

        // Update is called once per frame
        void Update() {
            if (currentValue < targetValue) {
                currentValue += Time.deltaTime * gainRate;
            } else {
                currentValue = targetValue;
            }
            UpdatePillar();
        }

        protected override void CardSelectedListener() {
            currentValue = 0;
        }

        protected override void NewCardDataListener() {
            targetValue = observedCard.data.GetValue(data.variable);
        }

        public void InitializePillar() {
            if (!data) {
                return;
            }
            label.text = data.variableName;
            icon.sprite = data.iconTransparent;
            value.color = data.color;
            bar.color = data.color;
            name = data.variableName + "Pillar";
            UpdatePillar();
        }

        void UpdatePillar() {
            value.text = ((int)currentValue).ToString();
            float valueShift = maximumBarWidth * (currentValue / maximumValue);
            bar.rectTransform.anchoredPosition = new Vector2(barOffset + valueShift / 2, 0);
            bar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, valueShift);
            value.rectTransform.anchoredPosition = new Vector2(valueOffset + valueShift, 0);
        }
    }
}
