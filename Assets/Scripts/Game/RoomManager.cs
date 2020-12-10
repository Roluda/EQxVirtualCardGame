using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;
using EQx.Game.Player;

namespace EQx.Game {
    public class RoomManager : MonoBehaviourPunCallbacks {
        [SerializeField]
        CardPlayer cardPlayerPrefab = default;

        private void Start() {
            PhotonNetwork.Instantiate(cardPlayerPrefab.name, Vector3.zero, Quaternion.identity);
        }

        public override void OnLeftRoom() {
            SceneManager.LoadScene(0);
        }

        public void LeaveRoom() {
            PhotonNetwork.LeaveRoom();
        }
    }
}
