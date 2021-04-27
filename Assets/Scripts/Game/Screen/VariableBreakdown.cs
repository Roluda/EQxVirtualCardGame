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
        GridLayoutGroup context = default;
        [SerializeField]
        Image icon = default;
        [SerializeField]
        TMP_Text head = default;
        [SerializeField]
        float displayInterval;

        [SerializeField]
        Vector2 subIndexCellSize = default;
        [SerializeField]
        Vector2 indexAreaCellSize = default;
        [SerializeField]
        Vector2 pillarCellSize = default;
        [SerializeField]
        Vector2 indicatorCellSize = default;

        List<GameObject> instances = new List<GameObject>();

        public void ShowIndicator() {
            StartCoroutine(ShowRoutine());
        }

        IEnumerator ShowRoutine() {
            var variable = RoundManager.instance.currentDemand;
            var variableData = EQxVariableDatabase.instance.GetVariable(variable);
            icon.sprite = variableData.iconWhite;
            head.text = FindHeader(variableData);
            context.cellSize = DetermineCellSize(variableData);
            if (variableData.level == EQxLevel.Pillar) {
                foreach (var indicator in variableData.indicators) {
                    yield return new WaitForSeconds(displayInterval);
                    var newEntry = Instantiate(entryPrefab, context.transform);
                    newEntry.color = variableData.color;
                    newEntry.indicatorName = indicator;
                    newEntry.iconEnabled = false;
                    instances.Add(newEntry.gameObject);
                }
            } else {
                foreach(var subVariable in variableData.subVariables) {
                    yield return new WaitForSeconds(displayInterval);
                    var newEntry = Instantiate(entryPrefab, context.transform);
                    newEntry.color = subVariable.color;
                    newEntry.indicatorName = subVariable.variableName;
                    newEntry.icon = subVariable.iconWhite;
                    instances.Add(newEntry.gameObject);
                }
            }
        }

        private Vector2 DetermineCellSize(EQxVariableData data) {
            switch (data.level) {
                case EQxLevel.Index:
                    return subIndexCellSize;
                case EQxLevel.SubIndex:
                    return indexAreaCellSize;
                case EQxLevel.IndexArea:
                    return pillarCellSize;
                case EQxLevel.Pillar:
                    return indicatorCellSize;
                default:
                    throw new NotImplementedException("This EQx Level has not children cell size");
            }
        }

        string FindHeader(EQxVariableData demand) {
            switch (demand.level) {
                case EQxLevel.Index:
                    return "The Sub-Indices of the EQx are:";
                case EQxLevel.SubIndex:
                    return $"The Index Areas for {demand.variableName} are:";
                case EQxLevel.IndexArea:
                    return $"The Pillars of {demand.variableName} are:";
                case EQxLevel.Pillar:
                    return $"The Indicators for {demand.variableName} are:";
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
