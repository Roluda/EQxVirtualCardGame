using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EQx.Game.Table {
    public class DemandVisualization : MonoBehaviour {

        [SerializeField]
        float typeInterval = 0.01f;
        [SerializeField]
        float waitAfterHead = 1;
        [SerializeField]
        float waitAfterStructure = 1;

        [SerializeField]
        Transform context = default;
        [SerializeField]
        Image icon = default;
        [SerializeField]
        TMP_Text head = default;
        [SerializeField]
        TMP_Text body = default;

        [SerializeField]
        VerticalLayoutGroup structureGroup = default;
        [SerializeField]
        Image pillarImage = default;

        [SerializeField]
        float closedImageHeight = 250;
        [SerializeField]
        float openImageHeight = 500;
        [SerializeField]
        float closedSpacing = -700;
        [SerializeField]
        float openSpacing = -200;

        [SerializeField]
        float structureAnimationTime = 1;

        private void Start() {
        }

        public void ShowDemand() {
            EQxVariableData demand = EQxVariableDatabase.instance.GetVariable(RoundManager.instance.currentDemand);
            StopAllCoroutines();
            StartCoroutine(PresentDemand(demand));
        }

        public void HideDemand() {
            StopAllCoroutines();
            StartCoroutine(RemoveDemand());
        }

        IEnumerator PresentDemand(EQxVariableData data) {
            context.gameObject.SetActive(true);
            structureGroup.spacing = closedSpacing;
            pillarImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, closedImageHeight);
            pillarImage.sprite = data.structureHihlighted;
            head.text = "";
            body.text = "";
            icon.enabled = false;
            StartCoroutine(ShowStructure());
            yield return new WaitForSeconds(waitAfterStructure);
            icon.enabled = true;
            icon.sprite = data.iconWhite;
            StartCoroutine(TypeText(head, $"{data.variableName}"));
            yield return new WaitForSeconds(waitAfterHead);
            StartCoroutine(TypeText(body, data.description));
        }

        IEnumerator RemoveDemand() {
            StartCoroutine(HideStructure());
            yield return new WaitForSeconds(structureAnimationTime*1.3f);
            context.gameObject.SetActive(false);
        }

        IEnumerator ShowStructure() {
            float t = 0;
            while (t < structureAnimationTime) {
                t += Time.deltaTime;
                structureGroup.spacing = Mathf.Lerp(closedSpacing, openSpacing, t / structureAnimationTime);
                pillarImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(closedImageHeight, openImageHeight, t / structureAnimationTime));
                yield return null;
            }
        }

        IEnumerator HideStructure() {
            float t = 0;
            while (t < structureAnimationTime) {
                t += Time.deltaTime;
                structureGroup.spacing = Mathf.Lerp(openSpacing, closedSpacing, t / structureAnimationTime);
                pillarImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(openImageHeight, closedImageHeight, t / structureAnimationTime));
                yield return null;
            }
        }

        IEnumerator TypeText(TMP_Text text, string content) {
            string displayContent = "";
            foreach (var letter in content) {
                displayContent += letter;
                text.text = displayContent;
                yield return new WaitForSeconds(typeInterval);
            }
        }
    }
}
