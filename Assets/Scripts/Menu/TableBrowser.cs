using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

namespace EQx.Menu {
    public class TableBrowser : MonoBehaviourPunCallbacks {
		[SerializeField, Range(0, 180)]
		float refreshInterval = 10;

		[SerializeField]
		string gameVersion = "Develop";
		[SerializeField]
		int maxPlayers = 6;

		[SerializeField]
		string hostGamePrefix = "Table of ";
		[SerializeField]
		string hostDefaultName = "Anonymous";

		[SerializeField]
		Transform content = null;
		[SerializeField]
		GameObject serverOption = null;

		float timer = 0;


		private void Start() {
			ConnectToPhoton();
		}

        private void ConnectToPhoton() {
            if (!PhotonNetwork.IsConnected) {
				PhotonNetwork.ConnectUsingSettings();
				PhotonNetwork.AutomaticallySyncScene = true;
				PhotonNetwork.GameVersion = gameVersion;
            }
        }

        public void CreateServerOption(string name, UnityEngine.Events.UnityAction callback) {
			var option = Instantiate(serverOption);
			option.transform.SetParent(content);
			var browserItem = option.GetComponent<TableOption>();
			if (browserItem != null)
				browserItem.SetData(name, callback);
		}

		public void Host() {
            if (!PhotonNetwork.IsConnected) {
				Debug.LogWarning("Not Cennected to PhotonNetwork");
				return;
            }
			string roomName = hostGamePrefix + PlayerPrefs.GetString(PlayerPrefKeys.PLAYERNAME, hostDefaultName);
			var roomOptions = new RoomOptions { MaxPlayers = (byte)maxPlayers };
			PhotonNetwork.CreateRoom(roomName, roomOptions);
		}

        public override void OnRoomListUpdate(List<RoomInfo> roomList) {
			for (int i = content.childCount - 1; i >= 0; --i)
				Destroy(content.GetChild(i).gameObject);

			foreach (var room in roomList) {
				CreateServerOption(room.Name, () => {
					PhotonNetwork.JoinRoom(room.Name);
				});
			}
		}

		public override void OnJoinedRoom() {
            if (PhotonNetwork.IsMasterClient) {
				PhotonNetwork.LoadLevel(1);
            }
        }

        public override void OnCreateRoomFailed(short returnCode, string message) {
			Debug.LogWarning(message);
        }

        public override void OnJoinRoomFailed(short returnCode, string message) {
			Debug.LogWarning(message);
        }

        private void OnApplicationQuit() {
			if (PhotonNetwork.IsConnected) {
				PhotonNetwork.Disconnect();
			}
        }
    }
}
