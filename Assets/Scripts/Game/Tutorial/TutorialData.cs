using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Tutorial {
    [Serializable]
    public class TutorialData {
        [TextArea]
        public string text;
        public Sprite picture;
    }
}
