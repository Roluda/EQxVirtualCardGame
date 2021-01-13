using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

namespace EQx.Game.Table {
    public class NewRoundButton : MonoBehaviour , IInRoomCallbacks{
        // Start is called before the first frame update
        bool inRound;

        void Start() {
            RoundManager.instance.onPlacingEnded += ActivateButton;
            RoundManager.instance.onPlacingStarted += () => inRound = true;
            if (!PhotonNetwork.IsMasterClient) {
                gameObject.SetActive(false);
            }
        }

        public void ActivateButton() {
            inRound = false;
            if (PhotonNetwork.IsMasterClient) {
                gameObject.SetActive(true);
            }
        }

        void IInRoomCallbacks.OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
        }

        void IInRoomCallbacks.OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) {
        }

        void IInRoomCallbacks.OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged) {
        }

        void IInRoomCallbacks.OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) {
        }

        void IInRoomCallbacks.OnMasterClientSwitched(Photon.Realtime.Player newMasterClient) {
            if (!inRound) {
                ActivateButton();
            }
        }
    }
}
