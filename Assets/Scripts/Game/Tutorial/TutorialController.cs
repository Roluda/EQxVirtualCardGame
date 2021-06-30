﻿using EQx.Game.Player;
using EQx.Game.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EQx.Game.Tutorial {
    public class TutorialController : MonoBehaviour {
        static bool firstOpen = true;

        bool roundEndedDataUnlocked = false;
        bool commitValueDataUnlocked = false;

        [SerializeField]
        Button tutorialButton = default;
        [SerializeField]
        TutorialSystem tutorialSystem = default;

        [SerializeField]
        List<TutorialData> initialData = new List<TutorialData>();
        [SerializeField]
        List<TutorialData> commitValueData = new List<TutorialData>();
        [SerializeField]
        List<TutorialData> roundEndedData = new List<TutorialData>();

        private void Awake() {
            CardPlayer.localPlayerReady += RegisterPlayerListeners;
        }

        private void RegisterPlayerListeners(CardPlayer player) {
            CardPlayer.localPlayerReady -= RegisterPlayerListeners;
            player.onEndedPlacing += _ => AddCommitValueData();
        }

        private void Start() {
            tutorialButton.onClick.AddListener(tutorialSystem.OpenTutorial);
            tutorialSystem.Unlock(initialData);
            RoundManager.instance.onNewRound += AddRoundEndedData;
        }

        public void OpenBasicTutorial() {
            if (firstOpen) {
                firstOpen = false;
                tutorialSystem.OpenTutorial();
            }
        }

        void AddCommitValueData() {
            if (!commitValueDataUnlocked) {
                commitValueDataUnlocked = true;
                tutorialSystem.Unlock(commitValueData);
                tutorialSystem.NextWindow();
            }
        }

        void AddRoundEndedData() {
            if(commitValueDataUnlocked && !roundEndedDataUnlocked) {
                roundEndedDataUnlocked = true;
                tutorialSystem.Unlock(commitValueData);
            }
        }        
    }
}
