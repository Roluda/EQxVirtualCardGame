using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Tutorial {
    [CreateAssetMenu(fileName="TD_New", menuName="TutorialData")]
    public class TutorialDataAsset : ScriptableObject {
        [SerializeField, TextArea(10, 20)]
        public string rawText = default;

        [SerializeField]
        public Sprite picture = default;

        public string escapeText => rawText.Replace("<bullet>", "\u2022");
    }
}
