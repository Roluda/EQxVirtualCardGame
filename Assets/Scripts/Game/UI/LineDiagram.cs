using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using System.Linq;
using System;
using TMPro;

namespace EQx.Game.UI {
    public class LineDiagram : MonoBehaviour {
        struct Line {
            public float xMin => unscaledPoints.Min(vector => vector.x);
            public float xMax => unscaledPoints.Max(vector => vector.x);
            public float yMin => unscaledPoints.Min(vector => vector.y);
            public float yMax => unscaledPoints.Max(vector => vector.y);
            public Vector2[] unscaledPoints;
            public string legendName;
            public Color color;
        }

        [Header("Reference Configuration")]
        [SerializeField]
        Transform lineContext = default;
        [SerializeField]
        UILineRenderer linePrefab = default;
        [SerializeField]
        Transform legendContext = default;
        [SerializeField]
        TMP_Text legendEntryPrefab = default;
        [SerializeField]
        Transform xScaleContext = default;
        [SerializeField]
        Transform yScaleContext = default;
        [SerializeField]
        TMP_Text scaleNumberPrefab = default;
        [SerializeField]
        TMP_Text headerObject = default;
        [SerializeField]
        TMP_Text xLabel = default;
        [SerializeField]
        TMP_Text yLabel = default;

        [Header("DesignConfiguration")]
        [SerializeField]
        ColorScheme colorScheme = default;
        [SerializeField]
        public int xScaleSegments = 2;
        [SerializeField]
        public int yScaleSegments = 2;
        [SerializeField]
        int xScaleDecimals = 1;
        [SerializeField]
        int yScaleDecimals = 1;

        [SerializeField]
        float minMargin = 0.9f;
        [SerializeField]
        float maxMargin = 1.1f;

        public string labelX { get => xLabel.text; set => xLabel.text = value; }
        public string labelY { get => yLabel.text; set => yLabel.text = value; }
        public string header { get => headerObject.text; set => headerObject.text = value; }

        float xMin => lineData.Min(data => data.xMin);
        float xMax => lineData.Max(data => data.xMax);
        float yMin => lineData.Min(data => data.yMin) * minMargin;
        float yMax => lineData.Max(data => data.yMax) * maxMargin;

        List<Line> lineData = new List<Line>();

        public void AddLine<T1, T2>(Dictionary<T1, T2> data, string name = "") where T1 : IConvertible where T2 : IConvertible{
            AddLineData(data, name);
            Redraw();
        }

        public void AddLineData<T1, T2>(Dictionary<T1, T2> data, string name = "") where T1 : IConvertible where T2 : IConvertible{
            if (data.Count < 2) {
                return;
            }
            var points = new List<Vector2>();
            data.ToList().ForEach(point => points.Add(new Vector2(point.Key.ToSingle(null), point.Value.ToSingle(null))));
            lineData.Add(new Line {
                unscaledPoints = points.ToArray(),
                legendName = name,
                color = colorScheme.GetColor(lineData.Count)
            });
        }

        public void Redraw() {
            Clear();
            DrawLines();
            DrawScale(xScaleContext, scaleNumberPrefab, xScaleSegments, xScaleDecimals, xMin, xMax, false);
            DrawScale(yScaleContext, scaleNumberPrefab, yScaleSegments, yScaleDecimals, yMin, yMax, true);
        }


        public void ClearData() {
            lineData.Clear();
            Clear();
        }

        public void Clear() {
            DestroyAllChildren(lineContext);
            DestroyAllChildren(legendContext);
            DestroyAllChildren(xScaleContext);
            DestroyAllChildren(yScaleContext);
        }

        void DrawLines() {
            foreach (var line in lineData) {
                var newLine = Instantiate(linePrefab, lineContext);
                newLine.Points = NormalizedPoints(line.unscaledPoints);
                newLine.color = line.color;
                if (line.legendName.Trim() != string.Empty) {
                    var legendEntry = Instantiate(legendEntryPrefab, legendContext);
                    legendEntry.text = line.legendName;
                    legendEntry.color = newLine.color;
                }
            }
        }

        void DrawScale(Transform context, TMP_Text prefab, int segments, int decimals, float min, float max, bool revert) {
            if (segments > 1) {
                for (int i = 0; i < segments; i++) {
                    float segmentSize = (max - min) / (segments - 1);
                    float segmentValue = min + i * segmentSize;
                    var text = Instantiate(prefab, context);
                    text.text = Math.Round(segmentValue, decimals).ToString();
                    if (revert) {
                        text.transform.SetAsFirstSibling();
                    }
                }
            }
        }

        private void DestroyAllChildren(Transform context) {
            for (int i= context.childCount -1; i>=0; i--) {
                Destroy(context.GetChild(i).gameObject);
            }
        }

        Vector2[] NormalizedPoints(Vector2[] unscaledPoints) {
            var copy = new Vector2[unscaledPoints.Length];
            for (int i = 0; i < unscaledPoints.Length; i++) {
                copy[i].x = (unscaledPoints[i].x - xMin) / (xMax - xMin);
                copy[i].y = (unscaledPoints[i].y - yMin) / (yMax - yMin);
            }
            return copy;
        }
    }
}
