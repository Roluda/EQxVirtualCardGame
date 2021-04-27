using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EQx.Game.Screen {
    public class BreakdownEntry : MonoBehaviour {
        [SerializeField]
        Image iconImage = default;
        [SerializeField]
        TMP_Text text = default;
        [SerializeField]
        Image background = default;
        [SerializeField]
        float imageMargin = 200;

        public string indicatorName { set => text.text = value; }

        public Color color {set => background.color = value; }

        public Sprite icon { set {
                iconImage.sprite = value;
                var currentMargin = text.margin;
                currentMargin.x += imageMargin;
                text.margin = currentMargin;
            }
        }

        public bool iconEnabled { set => iconImage.enabled = value; }
    }
}
