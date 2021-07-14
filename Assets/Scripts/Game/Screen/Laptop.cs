using EQx.Game.Player;
using EQx.Game.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Screen {
    public class Laptop : MonoBehaviour {
        [SerializeField]
        GameObject investmentInterface = default;
        [SerializeField]
        GameObject scoreboardInterface = default;


        public void InvestmentOn() {
            investmentInterface.gameObject.SetActive(true);
            scoreboardInterface.gameObject.SetActive(false);
        }

        public void ScoreboardOn() {
            investmentInterface.gameObject.SetActive(false);
            scoreboardInterface.gameObject.SetActive(true);
        }


        private void ActivateScoreboard(CardPlayer player, int round) {
            ScoreboardOn();
        }

        private void ActivateInvestment(CardPlayer player, int round) {
            InvestmentOn();
        }

        private void Awake() {
            CardPlayer.localPlayerReady += Init;
        }

        private void Init(CardPlayer player) {
            CardPlayer.localPlayerReady -= Init;
            player.onStartedBetting += ActivateInvestment;
            player.onStartedPlacing += ActivateScoreboard;
            player.onEndedBetting += ActivateScoreboard;
        }


        // Start is called before the first frame update
        void Start() {
            investmentInterface.gameObject.SetActive(false);
            scoreboardInterface.gameObject.SetActive(true);
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
