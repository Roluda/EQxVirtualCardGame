using UnityEngine;
using Photon.Pun;
using UnityEngine.Analytics;
using System.Collections.Generic;

namespace EQx.Analytics {
    public class RoomTracking : MonoBehaviour {
        // Start is called before the first frame update
        void Start() {
#if !UNITY_EDITOR
            if (PhotonNetwork.IsMasterClient) {
                AnalyticsEvent.GameStart(new Dictionary<string, object> { { "players", PhotonNetwork.CurrentRoom.PlayerCount } });
            }
#endif
        }
    }
}
