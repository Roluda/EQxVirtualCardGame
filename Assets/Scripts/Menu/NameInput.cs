using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EQx.Menu {
    public class NameInput : MonoBehaviour {
        [SerializeField]
        TMP_InputField nameField = default;
        [SerializeField]
        string defaultName = default;

        public void SetPlayerName(string name) {
            PlayerPrefs.SetString(PlayerPrefKeys.PLAYERNAME, name);
        }

        // Start is called before the first frame update
        void Start() {
            nameField.text = PlayerPrefs.GetString(PlayerPrefKeys.PLAYERNAME, defaultName);
        }
    }
}
