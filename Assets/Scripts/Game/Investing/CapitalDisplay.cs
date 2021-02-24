using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EQx.Game.Investing {
    public class CapitalDisplay : MonoBehaviour {
        [SerializeField]
        TMP_Text infoText = default;
        [SerializeField]
        string infoPrefix = "Your Elite Coins: ";
        [SerializeField]
        bool onlyShowWhenHighlighted = true;
        [SerializeField]
        PileMountain observedMountain = default;

        void Start() {
            if (onlyShowWhenHighlighted) {
                infoText.gameObject.SetActive(false);
            }
        }
        // Update is called once per frame
        void Update() {
            infoText.text = $"{infoPrefix}{observedMountain.capital}";
            if (onlyShowWhenHighlighted) {
                infoText.gameObject.SetActive(observedMountain.highlighted);
            }
        }
    }
}
