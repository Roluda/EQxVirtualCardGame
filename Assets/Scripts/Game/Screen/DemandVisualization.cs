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
        Image icon = default;
        [SerializeField]
        Image background = default;
        [SerializeField]
        TMP_Text head = default;
        [SerializeField]
        TMP_Text body = default;

        [SerializeField]
        RectTransform context;
        [SerializeField]
        Vector3 winScreenTranslation = new Vector3(0, 350, 0);
        [SerializeField]
        Vector3 winSceenScale = new Vector3(0.7f, 0.7f, 0.7f);
        [SerializeField, Range(0,10)]
        float translationDuration = 1;


        public Vector3 originalPosition;
        public Vector3 originalScale;
        Vector3 winScreenPosition;

        private void Start() {
            originalPosition = context.anchoredPosition3D;
            originalScale = context.localScale;
            winScreenPosition = originalPosition + winScreenTranslation;
        }

        public void ShowDemand() {
            EQxVariableData demand = EQxVariableDatabase.instance.GetVariable(RoundManager.instance.currentDemand);
            StartCoroutine(ChangeDemand(demand));
        }

        public void SetWinScreenScale() {
            StartCoroutine(TranslateScale(winSceenScale));
        }

        public void SetWinScreenPosition() {
            StartCoroutine(TranslatePosition(winScreenPosition));
        }

        public void SetNormalScreenScale() {
            StartCoroutine(TranslateScale(originalScale));
        }
        public void SetNormalScreenPosition() {
            StartCoroutine(TranslatePosition(originalPosition));
        }

        void NewDemandListener(EQxVariableType demand) {
            Debug.Log("NewDemandListener");
            StartCoroutine(ChangeDemand(EQxVariableDatabase.instance.GetVariable(demand)));
        }

        IEnumerator ChangeDemand(EQxVariableData data) {
            Debug.Log("data" + data);
            head.text = "";
            body.text = "";
            int round = RoundManager.instance.currentRound;
            int max = RoundManager.instance.maxRounds;
            icon.sprite = data.iconWhite;
            background.color = data.color;
            StartCoroutine(TypeText(head, $"Round {round}/{max} - {data.variableName}"));
            yield return new WaitForSeconds(waitAfterHead);
            StartCoroutine(TypeText(body, data.description));
        }

        IEnumerator TranslatePosition(Vector3 targetPosition) {
            float timer = 0;
            float moveSpeed = (context.anchoredPosition3D - targetPosition).magnitude / translationDuration;
            while (timer < translationDuration) {
                timer += Time.deltaTime;
                context.anchoredPosition3D = Vector3.MoveTowards(context.anchoredPosition3D, targetPosition, moveSpeed*Time.deltaTime);
                yield return null;
            }
            context.anchoredPosition3D = targetPosition;
        }

        IEnumerator TranslateScale(Vector3 targetScale) {
            float timer = 0;
            float growSpeed = (context.localScale - targetScale).magnitude / translationDuration;
            while (timer < translationDuration) {
                timer += Time.deltaTime;
                context.localScale = Vector3.MoveTowards(context.localScale, targetScale, growSpeed * Time.deltaTime);
                yield return null;
            }
            context.localScale = targetScale;
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
