using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game {
    [CreateAssetMenu(fileName = "CS_New", menuName = "ColorScheme", order =0)]
    public class ColorScheme : ScriptableObject {
        [SerializeField]
        Color[] colors = default;

        public Color GetColor(int index) {
            if(index < colors.Length) {
                return colors[index];
            } else {
                return Color.white;
            }
        }
    }
}
