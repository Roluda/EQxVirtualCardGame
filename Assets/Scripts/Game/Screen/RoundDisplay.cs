using EQx.Game.Table;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EQx.Game.Screen {
    public class RoundDisplay : MonoBehaviour {

        [SerializeField]
        TMP_Text text = default;
        [SerializeField]
        string roundPrefix = "Round ";


        public void ShowCurrentRound() {
            text.gameObject.SetActive(true);
            text.text = $"{roundPrefix}{RoundManager.instance.currentRound}/{RoundManager.instance.maxRounds}";
        }

        public void HideCurrentRound() {
            text.gameObject.SetActive(false);
        }
    }
}
