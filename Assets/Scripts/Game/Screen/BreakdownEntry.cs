using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EQx.Game.Screen {
    public class BreakdownEntry : MonoBehaviour {

        [SerializeField]
        TMP_Text text = default;
        [SerializeField]
        Image background = default;

        public string indicatorName { get => text.text; set => text.text = value; }

        public Color color { get => background.color; set => background.color = value; }
    }
}
