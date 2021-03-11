using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EQx.Menu {
    public class HostOption : MonoBehaviour {
        [SerializeField]
        TableBrowser browser = default;
        [SerializeField]
        TMP_InputField input = default;
        [SerializeField]
        Slider slider = default;
        [SerializeField]
        TMP_Text playersText = default;

        [SerializeField]
        string playerTextPrefix = "Maximum Players: ";
        [SerializeField]
        TMP_Text nameErrorMessage = default;
        [SerializeField]
        string nameNonUniqueMessage = "Name must be unique!";
        [SerializeField]
        string nameEmptyMessage = "Name can't be empty!";

        [SerializeField]
        Slider maxRooms = default;
        [SerializeField]
        TMP_Text maxRoundsText = default;
        [SerializeField]
        string maxRoundsPrefix = "Rounds to play: ";

        private void Start() {
            browser.onRoomNotUnique += ShowNonUniqueMessage;
            if (PlayerPrefs.HasKey(PlayerPrefKeys.MAXPLAYERS)) {
                slider.value = PlayerPrefs.GetInt(PlayerPrefKeys.MAXPLAYERS);
            }
            if (PlayerPrefs.HasKey(PlayerPrefKeys.MAXROUNDS)) {
                maxRooms.value = PlayerPrefs.GetInt(PlayerPrefKeys.MAXROUNDS);
            }
            if (PlayerPrefs.HasKey(PlayerPrefKeys.ROOMNAME)) {
                input.text = PlayerPrefs.GetString(PlayerPrefKeys.ROOMNAME);
            }
        }

        private void ShowNonUniqueMessage() {
            nameErrorMessage.gameObject.SetActive(true);
            nameErrorMessage.text = nameNonUniqueMessage;
        }

        public void ConfirmHost() {
            if(input.text.Trim() == "") {
                nameErrorMessage.gameObject.SetActive(true);
                nameErrorMessage.text = nameEmptyMessage;
            } else {
                PlayerPrefs.SetString(PlayerPrefKeys.ROOMNAME, input.text);
                PlayerPrefs.SetInt(PlayerPrefKeys.MAXPLAYERS, (int)slider.value);
                PlayerPrefs.SetInt(PlayerPrefKeys.MAXROUNDS, (int)maxRooms.value);
                browser.Host(input.text, (int)slider.value, (int)maxRooms.value);

            }
        }

        public void SliderUpdate(float amount) {
            playersText.text = playerTextPrefix + (int)amount;
        }

        public void RoundSliderUpdate(float amount) {
            maxRoundsText.text = maxRoundsPrefix + (int)amount;
        }
    }
}
