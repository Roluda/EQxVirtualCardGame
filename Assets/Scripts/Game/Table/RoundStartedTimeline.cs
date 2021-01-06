using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Table {
    public class RoundStartedTimeline : CallTimeline {
        // Start is called before the first frame update
        void Start() {
            RoundManager.instance.onPlacingStarted += Play;
            RoundManager.instance.onPlacingEnded += Stop;
        }
    }
}
