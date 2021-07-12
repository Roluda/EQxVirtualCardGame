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
        [SerializeField]
        float popupSpeed = 5;
        [SerializeField]
        float promptDelay = 0.3f;

        public void Close() {
            isOpen = false;
            gameObject.SetActive(false);
        }

        bool isOpen = false;

        public void Open(TutorialDataAsset data, bool hasPrevious, bool hasNext) {
            previousButton.gameObject.SetActive(hasPrevious);
            nextButton.gameObject.SetActive(hasNext);
            okButton.gameObject.SetActive(!hasNext);
            if (isOpen) {
                StopAllCoroutines();
                StartCoroutine(ShowData(data));
                transform.localScale = Vector3.one;
            } else {
                isOpen = true;
                gameObject.SetActive(true);
                transform.localScale = Vector3.zero;
                StopAllCoroutines();
                StartCoroutine(OpenWindow(data));
            }
        }

        IEnumerator OpenWindow(TutorialDataAsset data) {
            picture.enabled = false;
            bodyText.text = string.Empty;
            while(transform.localScale.x < 1) {
                transform.localScale += Vector3.one * Time.deltaTime * popupSpeed;
                yield return null;
            }
            transform.localScale = Vector3.one;
            yield return new WaitForSeconds(promptDelay);
            StartCoroutine(ShowData(data));
        }

        IEnumerator ShowData(TutorialDataAsset data) {
            picture.enabled = data.picture;
            picture.sprite = data.picture;
            bodyText.text = string.Empty;
            var textCache = data.escapeText.Split(' ');
            foreach(var letter in textCache) {
                bodyText.text += letter+" ";
                yield return new WaitForSeconds(typeInterval);
            }
        }
    }
}
