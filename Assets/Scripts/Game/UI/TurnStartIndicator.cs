using EQx.Game.Audio;
using EQx.Game.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace EQx.Game.LocalPlayer {
    public class TurnStartIndicator : MonoBehaviour {
        [SerializeField]
        Image background = default;
        [SerializeField]
        GameObject textObject = default;
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

        private void Awake() {
            CardPlayer.localPlayerReady += Initialize;
            textObject.SetActive(false);
        }

        private void Initialize(CardPlayer player) {
            CardPlayer.localPlayerReady -= Initialize;
            player.onStartedTurn += StartedTurnListener;
        }

        private void StartedTurnListener(CardPlayer player) {
            StartCoroutine(DisplayIndication());
        }

        IEnumerator DisplayIndication() {
            float time = 0;
            Vignette vignette;
            vignetteVolume.profile.TryGet(out vignette);
            bool playedNotification = false;
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
                    textObject.SetActive(true);
                } else {
                    textObject.SetActive(false);
                }
                yield return null;
            }
            textObject.SetActive(false);
        }
    }
}
