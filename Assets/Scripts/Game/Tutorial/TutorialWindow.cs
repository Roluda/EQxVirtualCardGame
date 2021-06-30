using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EQx.Game.Tutorial {
    public class TutorialWindow : MonoBehaviour {
        public Button previousButton = default;
        public Button nextButton = default;
        public Button okButton = default;
        public TMP_Text bodyText = default;
        public Image picture = default;

        [SerializeField]
        float typeInterval = 0.05f;

        private void Awake() {
            gameObject.SetActive(false);
        }

        public void Close() {
            gameObject.SetActive(false);
        }

        public void Open(TutorialData data, bool hasPrevious, bool hasNext) {
            gameObject.SetActive(true);
            previousButton.gameObject.SetActive(hasPrevious);
            nextButton.gameObject.SetActive(hasNext);
            StopAllCoroutines();
            StartCoroutine(ShowData(data));
        }

        IEnumerator ShowData(TutorialData data) {
            picture.enabled = data.picture;
            picture.sprite = data.picture;
            bodyText.text = string.Empty;
            foreach(var letter in data.text) {
                bodyText.text += letter;
                yield return new WaitForSeconds(typeInterval);
            }
        }
    }
}
