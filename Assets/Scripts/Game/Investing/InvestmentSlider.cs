using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EQx.Game.Investing {
    public class InvestmentSlider : MonoBehaviour {
        public UnityAction<int> onCommitmentUpdate;

        [SerializeField]
        Slider slider = default;
        [SerializeField]
        Image fill = default;
        [SerializeField]
        Color extractionColor = Color.yellow;
        [SerializeField]
        Color investmentColor = Color.blue;

        int maxExtraction => InvestmentManager.instance.maxExtraction;
        int maxInvestment => InvestmentManager.instance.maxCreation;
        float segment;

        public void Reset() {
            slider.value = 0;
        }

        void UpdateSlider(float value) {
            onCommitmentUpdate?.Invoke((int)value);
            float size = Mathf.Abs(value) * segment;


            if (value < 0) {
                fill.color = extractionColor;
                fill.rectTransform.anchoredPosition = new Vector2((maxExtraction + value) * segment + size / 2, 0);
            } else {
                fill.color = investmentColor;
                fill.rectTransform.anchoredPosition = new Vector2((maxExtraction + value) * segment - size / 2, 0);
            }
            fill.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
        }

        // Start is called before the first frame update
        void Start() {
            slider.wholeNumbers = true;
            slider.minValue = -maxExtraction;
            slider.maxValue = maxInvestment;
            slider.onValueChanged.AddListener(UpdateSlider);
            segment = slider.GetComponent<RectTransform>().rect.width / (maxExtraction + maxInvestment);
        }
    }
}
