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
        public void SetData(string sender, string message) {
            senderText.text = sender + ":";
            messageText.text = message;
            if (chatPopSound) {
                chatPopSound.Play();
            }
        }

        private void Start() {
            var scrollRect = GetComponentInParent<ScrollRect>();
            if (scrollRect != null) {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 0;
            }
        }
    }
}
