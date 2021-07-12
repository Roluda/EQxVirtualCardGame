using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EQx.Game.Tutorial {
    public class TutorialSystem : MonoBehaviour {
        public static bool inTutorial = false;

        [SerializeField]
        TutorialWindow tutorialWindow = default;
        List<TutorialDataAsset> tutorials = new List<TutorialDataAsset>();


        int currentIndex = 0;


        public void NextWindow() { 
            if(NextAvailable()) {
                currentIndex++;
                inTutorial = true;
                tutorialWindow.Open(tutorials[currentIndex], true, NextAvailable());
            }
        }

        public void PreviousWindow() {
            if (PreviousAvailable()) {
                currentIndex--;
                inTutorial = true;
                tutorialWindow.Open(tutorials[currentIndex], PreviousAvailable(), true);
            }
        }

        public void ExitTutorial() {
            inTutorial = false;
            tutorialWindow.Close();
        }

        public void OpenTutorial() {
            if (currentIndex < tutorials.Count) {
                inTutorial = true;
                tutorialWindow.Open(tutorials[currentIndex], PreviousAvailable(), NextAvailable());
            }
        }

        public void Unlock(TutorialDataAsset data) {
            tutorials.Add(data);
        }

        public void Unlock(List<TutorialDataAsset> data) {
            tutorials.AddRange(data);
        }

        bool NextAvailable() {
            return currentIndex < tutorials.Count - 1;
        }

        bool PreviousAvailable() {
            return currentIndex > 0;
        }

        private void Start() {
            inTutorial = false;
            tutorialWindow.nextButton.onClick.AddListener(NextWindow);
            tutorialWindow.previousButton.onClick.AddListener(PreviousWindow);
            tutorialWindow.okButton.onClick.AddListener(ExitTutorial);
        }
    }
}
