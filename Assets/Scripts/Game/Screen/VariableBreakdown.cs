using EQx.Game.Table;
using System;
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
            head.text = FindHeader(variableData);
            if (variableData.level == EQxLevel.Pillar) {
                foreach (var indicator in variableData.indicators) {
                    yield return new WaitForSeconds(displayInterval);
                    var newEntry = Instantiate(entryPrefab, context);
                    newEntry.color = variableData.color;
                    newEntry.indicatorName = indicator;
                    newEntry.iconEnabled = false;
                    instances.Add(newEntry.gameObject);
                }
            } else {
                foreach(var subVariable in variableData.subVariables) {
                    yield return new WaitForSeconds(displayInterval);
                    var newEntry = Instantiate(entryPrefab, context);
                    newEntry.color = subVariable.color;
                    newEntry.indicatorName = subVariable.variableName;
                    newEntry.icon = subVariable.iconWhite;
                    instances.Add(newEntry.gameObject);
                }
            }
        }

        string FindHeader(EQxVariableData demand) {
            switch (demand.level) {
                case EQxLevel.Index:
                    return "The sub indeces of the EQx are:";
                case EQxLevel.SubIndex:
                    return $"The index areas for {demand.variableName} are:";
                case EQxLevel.IndexArea:
                    return $"The pillars of {demand.variableName} are:";
                case EQxLevel.Pillar:
                    return $"The indicators for {demand.variableName} are:";
                default:
                    throw new NotImplementedException("this level has no head");
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
