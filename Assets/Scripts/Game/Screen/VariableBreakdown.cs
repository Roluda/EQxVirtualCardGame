using EQx.Game.Table;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EQx.Game.Screen {
    public class VariableBreakdown : MonoBehaviour {

        [SerializeField]
        BreakdownEntry entryPrefab = default;
        [SerializeField]
        Transform context = default;
        [SerializeField]
        Image icon = default;
        [SerializeField]
        TMP_Text head = default;
        [SerializeField]
        string headAffix = " is indicated by;";
        [SerializeField]
        float displayInterval;

        List<GameObject> instances = new List<GameObject>();

        public void ShowIndicator() {
            StartCoroutine(ShowRoutine());
        }

        IEnumerator ShowRoutine() {
            var variable = RoundManager.instance.currentDemand;
            var variableData = EQxVariableDatabase.instance.GetVariable(variable);
            icon.sprite = variableData.iconWhite;
            foreach(var indicator in variableData.indicators) {
                yield return new WaitForSeconds(displayInterval);
                var newEntry = Instantiate(entryPrefab, context);
                newEntry.color = variableData.color;
                newEntry.indicatorName = indicator;
                instances.Add(newEntry.gameObject);
            }
        }

        public void CleanUp() {
            StopAllCoroutines();
            foreach(var instance in instances) {
                Destroy(instance.gameObject);
            }
            instances.Clear();
        }
    }
}
