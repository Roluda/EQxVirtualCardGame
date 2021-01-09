﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EQx.Game.Investing {
    public class ValueSlider : MonoBehaviour {
        [SerializeField]
        Image bar = default;
        [SerializeField]
        float colorSaturationChange = 0;
        [SerializeField, Range(0, 10)]
        float lerpSpeed = 1;
        [SerializeField]
        float maxValue = 100;
        [SerializeField]
        float maximumBarWidth = 300;
        [SerializeField]
        float barOffset = 50;



        public Color color {
            get => bar.color;
            set {
                Color.RGBToHSV(value, out float h, out float s, out float v);
                v += colorSaturationChange;
                bar.color = Color.HSVToRGB(h, s, v);
            }
        }

        public void SetValue(float target) {
            targetValue = target;
        }

        public void SetValueInstant(float target) {
            currentValue = target;
            targetValue = target;
        }

        float targetValue;

        float currentValue;

        private void Update() {
            currentValue = Mathf.Lerp(currentValue, targetValue, Time.deltaTime * lerpSpeed);
            UpdatePillar();
        }

        void UpdatePillar() {
            float valueShift = maximumBarWidth * (currentValue / maxValue);
            bar.rectTransform.anchoredPosition = new Vector2(barOffset + valueShift / 2, 0);
            bar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, valueShift);

        }
    }
}
