using EQx.Game.Player;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EQx.Game.Screen {
    public class PrizeEntry : MonoBehaviour {
        [SerializeField]
        TMP_Text rankText = default;
        [SerializeField]
        TMP_Text nameText = default;
        [SerializeField]
        TMP_Text valueText = default;
        [SerializeField]
        TMP_Text gainText = default;
        [SerializeField]
        string valueTextPrefix = "Value: ";
        [SerializeField]
        Image avatarIcon = default;
        [SerializeField]
        Color positiveColor = Color.green;
        [SerializeField]
        Color negativeColor = Color.red;
        [SerializeField]
        Color creationColor = Color.blue;
        [SerializeField]
        Color extractionColor = Color.yellow;
        [SerializeField]
        float valueGainInterval = 0.1f;

        public int rank;

        public void SetRank(int rank) {
            rankText.text = rank+".";
            this.rank = rank;
        }

        public void SetName(string name) {
            nameText.text = name;
        }

        public void SetIcon(Sprite icon) {
            avatarIcon.sprite = icon;
        }

        public void SetValueInstant(int value) {
            currentValue = value;
            valueText.text = $"{valueTextPrefix}{value}";
        }

        public void SetValueLerp(int value) {
            StartCoroutine(SetValueAnimated(value));
        }

        public void SetCommitment(int value) {
            if (value > 0) {
                gainText.text = $"+{value}";
                gainText.color = extractionColor;
            } else if (value < 0) {
                gainText.text = $"{value}";
                gainText.color = creationColor;
            } else {
                gainText.text = "";
            }
        }

        public void SetGain(int value) {
            if(value > 0) {
                gainText.text = $"+{value}";
                gainText.color = positiveColor;
            } else if(value < 0) {
                gainText.text = $"{value}";
                gainText.color = negativeColor;
            } else {
                gainText.text = "";
            }
        }

        int currentValue = 0;

        IEnumerator SetValueAnimated(int target) {
            while (currentValue != target) {
                yield return new WaitForSeconds(valueGainInterval);
                if (currentValue < target) {
                    SetValueInstant(currentValue + 1);
                } else {
                    SetValueInstant(currentValue - 1);
                }
            }
        }

        // Start is called before the first frame update
        void Start() {
            gainText.text = "";
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
