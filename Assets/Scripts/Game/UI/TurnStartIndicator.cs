using EQx.Game.Audio;
using EQx.Game.Player;
using EQx.Game.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace EQx.Game.LocalPlayer {
    public class TurnStartIndicator : MonoBehaviour {
        [SerializeField]
        Image background = default;
        [SerializeField]
        TMP_Text textObject = default;
        [SerializeField]
        Volume vignetteVolume = default;
        [SerializeField]
        float displayDuration = 3;
        [SerializeField]
        float textDelay = 0.3f;
        [SerializeField]
        float textPrelay = 0.1f;
        [SerializeField]
        AnimationCurve vignetteOverDuration = default;
        [SerializeField]
        AnimationCurve backgroundAlphaOverDuration = default;
        [SerializeField]
        RandomSFX notification = default;
        [SerializeField]
        string placingMessage = "Place a Card";
        [SerializeField]
        string bettingMessage = "Commit Value";

        private void Awake() {
            CardPlayer.localPlayerReady += Initialize;
            textObject.gameObject.SetActive(false);
        }

        private void Initialize(CardPlayer player) {
            CardPlayer.localPlayerReady -= Initialize;
            player.onStartedPlacing += StartedPlacingListener;
            player.onStartedBetting += StartedBettingListener;
        }

        private void StartedBettingListener(CardPlayer player) {
            StartCoroutine(DisplayIndication(bettingMessage));
        }

        private void StartedPlacingListener(CardPlayer player) {
            StartCoroutine(DisplayIndication(placingMessage));
        }

        IEnumerator DisplayIndication(string message) {
            float time = 0;
            Vignette vignette;
            vignetteVolume.profile.TryGet(out vignette);
            bool playedNotification = false;
            bool clicked = false;
            while (time < displayDuration) {
                time += Time.deltaTime;
                var color = background.color;
                color.a = backgroundAlphaOverDuration.Evaluate(time/displayDuration);
                background.color = color;
                if (vignette) {
                    vignette.intensity.value = vignetteOverDuration.Evaluate(time / displayDuration);
                }
                if (time >= textDelay && time <= displayDuration-textPrelay) {
                    if (!playedNotification) {
                        notification.Play();
                        playedNotification = true;
                    }
                    textObject.text = message;
                    textObject.gameObject.SetActive(true);
                    while (clicked == false) {
                        time += Time.deltaTime;
                        clicked = Input.GetMouseButtonDown(0);
                        yield return null;
                    }
                } else {
                    textObject.gameObject.SetActive(false);
                }
                yield return null;
            }
            textObject.gameObject.SetActive(false);
        }
    }
}
