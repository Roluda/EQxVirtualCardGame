using EQx.Game.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EQx.Game.Player {
    public class DropZone : MonoBehaviour {

        [SerializeField]
        MouseOverDetector zoneArea = default;
        [SerializeField]
        Image background = default;

        [SerializeField]
        Color targetBackgroundColor = default;
        [SerializeField, Range(0, 2)]
        float fadeDuration = 0.3f;


        public bool hovered => zoneArea.isMouseOver;



        public void Show() {
            StopAllCoroutines();
            StartCoroutine(ZoneOn());
        }

        public void Hide() {
            StopAllCoroutines();
            StartCoroutine(ZoneOff());
        }

        IEnumerator ZoneOn() {
            float t = 0;
            while (t < fadeDuration) {
                t += Time.deltaTime;
                background.color = Color.Lerp(background.color, targetBackgroundColor, t / fadeDuration);
                yield return null;
            }
            zoneArea.gameObject.SetActive(true);
        }

        IEnumerator ZoneOff() {
            float t = 0;
            zoneArea.gameObject.SetActive(false);
            while (t < fadeDuration) {
                t += Time.deltaTime;
                background.color = Color.Lerp(background.color, Color.clear, t / fadeDuration);
                yield return null;
            }
        }
    }
}
