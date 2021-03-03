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


        private void EndedBettingListener(CardPlayer player) {
            ScoreboardOn();
        }

        private void EndedPlacingListener(CardPlayer player) {
            InvestmentOn();
        }

        private void Awake() {
            CardPlayer.localPlayerReady += Init;
        }

        private void Init(CardPlayer player) {
            CardPlayer.localPlayerReady -= Init;
            player.onEndedPlacing += EndedPlacingListener;
            player.onEndedBetting += EndedBettingListener;
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
