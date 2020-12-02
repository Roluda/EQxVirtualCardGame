﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game {
    [CreateAssetMenu(fileName = "EQxVar_New", menuName = "Data/EQxVariable", order = 1)]
    public class EQxVariableData : ScriptableObject {
        [SerializeField]
        public EQxVariable variable = default;
        [SerializeField]
        public string variableName = default;
        [SerializeField]
        public EQxLevel level = default;
        [SerializeField]
        public Color color = default;
        [SerializeField]
        public Sprite iconTransparent = default;
        [SerializeField]
        public Sprite iconWhite = default;
        [SerializeField]
        public Sprite iconBlack = default;
        [SerializeField]
        public string description = default;
    }
}
