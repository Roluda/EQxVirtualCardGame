using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.UI {
    public class LineDiagramTest : MonoBehaviour {

        [SerializeField]
        Vector2Int keyRange = new Vector2Int(0, 30);
        [SerializeField]
        Vector2 valueRange = new Vector3(0, 40);
        [SerializeField]
        int lines = 3;
        [SerializeField]
        LineDiagram diagram = default;

        public void DrawDiagram() {
            diagram.ClearData();
            for (int i = 0; i < lines; i++) {
                var data = new Dictionary<int, float>();
                for (int j = keyRange.x; j < keyRange.y; j++) {
                    data[j] = UnityEngine.Random.Range(valueRange.x, valueRange.y);
                }
                diagram.AddLineData(data, name = $"Test {i}");
            }
            diagram.Redraw();
        }
    }
}
