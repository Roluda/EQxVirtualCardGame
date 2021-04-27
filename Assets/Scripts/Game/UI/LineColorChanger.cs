using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace EQx.Game.UI {
    public class LineColorChanger : MonoBehaviour {
        [SerializeField]
        MouseOverDetector detector = default;
        [SerializeField]
        UILineRenderer lineRenderer = default;

        [SerializeField]
        Color defaultColor = Color.white;
        [SerializeField]
        Color highlightColor = Color.green;

        // Update is called once per frame
        void Update() {
            lineRenderer.color = detector.isMouseOver ? highlightColor : defaultColor;
        }
    }
}
