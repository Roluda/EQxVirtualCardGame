using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Table {
    public class RoundEndedTimeline : CallTimeline {
        // Start is called before the first frame update
        void Start() {
            RoundManager.instance.onPlacingEnded += Play;
            RoundManager.instance.onRoundStarted += Stop;
        }
    }
}
