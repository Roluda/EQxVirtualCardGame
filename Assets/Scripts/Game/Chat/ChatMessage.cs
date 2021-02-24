using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EQx.Game.Audio;
using UnityEngine.UI;

namespace EQx.Game.Chat {
    public class ChatMessage : MonoBehaviour {
        [SerializeField]
        TMP_Text senderText = default;
        [SerializeField]
        TMP_Text messageText = default;
        [SerializeField]
        RandomSFX chatPopSound = default;

        [SerializeField]
        AnimationCurve alphaOverDisplayTime = default;
        [SerializeField]
        float displayDuration = default;
        [SerializeField]
        Color chatColor = default;

        public float lifetime = 0;
        ScrollRect scrollRect;

        public void SetData(string sender, string message) {
            senderText.text = sender + ":";
            messageText.text = message;
            if (chatPopSound) {
                chatPopSound.Play();
            }
            lifetime = 0;
        }

        public void ResetDuration() {
            lifetime = 0;
        }

        private void Start() {
            scrollRect = GetComponentInParent<ScrollRect>();
            if (scrollRect != null) {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0;
            }
        }

        private void Update() {
            lifetime += Time.deltaTime;
            var displayColor = chatColor;
            displayColor.a = alphaOverDisplayTime.Evaluate(lifetime / displayDuration);
            senderText.color = displayColor;
            messageText.color = displayColor;
        }
    }
}
