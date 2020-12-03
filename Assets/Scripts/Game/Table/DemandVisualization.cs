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
        bool test = false;

        private void OnValidate() {
            if (test) {
                test = false;
                NewDemandListener(EQxVariableType.FirmDominace);
            }
        }

        private void Start() {
            GameTable.instance.onNewDemand += NewDemandListener;
        }

        private void OnDisable() {
            GameTable.instance.onNewDemand -= NewDemandListener;
        }

        void NewDemandListener(EQxVariableType demand) {
            StopAllCoroutines();
            StartCoroutine(ChangeDemand(EQxVariableDatabase.instance.GetVariable(demand)));
        }

        IEnumerator ChangeDemand(EQxVariableData data) {
            Debug.Log("data" + data);
            head.text = "";
            body.text = "";
            icon.sprite = data.iconWhite;
            background.color = data.color;
            StartCoroutine(TypeText(head, data.variableName));
            yield return new WaitForSeconds(waitAfterHead);
            StartCoroutine(TypeText(body, data.description));
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
