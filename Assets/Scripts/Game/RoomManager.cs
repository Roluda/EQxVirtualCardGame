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

        [SerializeField]
        bool kickAfk = true;
        [SerializeField]
        float timeUntilAfk = 90;

        float timeOfAFK;

        bool leaving = false;

        private void Start() {
            PhotonNetwork.Instantiate(cardPlayerPrefab.name, Vector3.zero, Quaternion.identity);
            timeOfAFK = Time.time;
        }

        private void Update() {
            if (Input.anyKey || Input.mouseScrollDelta != Vector2.zero) {
                timeOfAFK = Time.time;
            }
            float afkTime = Time.time - timeOfAFK;

            if(afkTime > timeUntilAfk && !leaving) {
                leaving = true;
                LeaveRoom();
            }
        }


        public override void OnLeftRoom() {
            SceneManager.LoadScene(0);
        }

        public void LeaveRoom() {
            PhotonNetwork.LeaveRoom();
        }
    }
}
