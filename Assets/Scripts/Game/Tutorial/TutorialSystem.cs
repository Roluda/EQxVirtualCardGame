using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EQx.Game.Tutorial {
    public class TutorialSystem : MonoBehaviour {
        [SerializeField]
        TutorialWindow tutorialWindow = default;
        List<TutorialData> tutorials = new List<TutorialData>();


        int currentIndex = 0;


        public void NextWindow() { 
            if(NextAvailable()) {
                currentIndex++;
                tutorialWindow.Open(tutorials[currentIndex], true, NextAvailable());
            }
        }

        public void PreviousWindow() {
            if (PreviousAvailable()) {
                currentIndex--;
                tutorialWindow.Open(tutorials[currentIndex], PreviousAvailable(), true);
            }
        }

        public void ExitTutorial() {
            tutorialWindow.Close();
        }

        public void OpenTutorial() {
            if (currentIndex < tutorials.Count) {
                tutorialWindow.Open(tutorials[currentIndex], PreviousAvailable(), NextAvailable());
            }
        }

        public void Unlock(TutorialData data) {
            tutorials.Add(data);
        }

        public void Unlock(List<TutorialData> data) {
            tutorials.AddRange(data);
        }

        bool NextAvailable() {
            return currentIndex < tutorials.Count - 1;
        }

        bool PreviousAvailable() {
            return currentIndex > 0;
        }

        private void Start() {
            tutorialWindow.nextButton.onClick.AddListener(NextWindow);
            tutorialWindow.previousButton.onClick.AddListener(PreviousWindow);
            tutorialWindow.okButton.onClick.AddListener(ExitTutorial);
        }
    }
}
