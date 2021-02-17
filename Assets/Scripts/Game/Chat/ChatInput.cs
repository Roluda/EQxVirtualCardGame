using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EQx.Game.Chat {
    public class ChatInput : MonoBehaviour {
        // Start is called before the first frame update
        [SerializeField]
        TMP_InputField inputField = default;

        void Start() {

        }

        // Update is called once per frame
        void Update() {
            if (Input.GetKeyDown(KeyCode.Return)) {
                if (!inputField.isFocused) {
                    inputField.ActivateInputField();
                }
            }
        }
    }
}
