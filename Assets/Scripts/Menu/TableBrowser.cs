using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

namespace EQx.Menu {
    public class TableBrowser : MonoBehaviourPunCallbacks {
		[SerializeField]
		string gameVersion = "Develop";

		[SerializeField]
		Transform content = null;
		[SerializeField]
		GameObject serverOption = null;

		public Action onRoomNotUnique;

		static string userID = "";

        public void ConnectToPhoton() {
            if (!PhotonNetwork.IsConnected) {
				if (!string.IsNullOrEmpty(userID)) {
					PhotonNetwork.AuthValues.UserId = userID;
				}
				PhotonNetwork.ConnectUsingSettings();
				PhotonNetwork.AutomaticallySyncScene = true;
				PhotonNetwork.GameVersion = gameVersion;
            }
        }

        private void Start() {
			ConnectToPhoton();
        }

        public override void OnConnectedToMaster() {
			userID = PhotonNetwork.LocalPlayer.UserId;
			Debug.Log("ConnectedToMaster");
			PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby() {
			Debug.Log("ConnectedToLobby");
        }

        public void CreateServerOption(string name, int currentPlayers, int maxPlayers, UnityEngine.Events.UnityAction callback) {
			var option = Instantiate(serverOption);
			option.transform.SetParent(content);
			var browserItem = option.GetComponent<TableOption>();
			if (browserItem != null)
				browserItem.SetData(name, currentPlayers, maxPlayers, callback);
		}

		public void Host(string roomName, int maxPlayers, int maxRounds) {
            if (!PhotonNetwork.IsConnected) {
				Debug.LogWarning("Not Cennected to PhotonNetwork");
				return;
            }
			var customProps = new ExitGames.Client.Photon.Hashtable();
			customProps["r"] = maxRounds;
			var roomOptions = new RoomOptions { MaxPlayers = (byte)maxPlayers , PlayerTtl = 0, PublishUserId = true, CustomRoomProperties = customProps};
			PhotonNetwork.CreateRoom(roomName, roomOptions);
		}

		public void JoinRandomRoom() {
            if (!PhotonNetwork.IsConnected) {
				return;
            }
			PhotonNetwork.JoinRandomRoom();
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList) {
			Debug.Log("Updated Room List");
			for (int i = content.childCount - 1; i >= 0; --i)
				Destroy(content.GetChild(i).gameObject);

			foreach (var room in roomList) {
				CreateServerOption(room.Name, room.PlayerCount, room.MaxPlayers, () => {
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
			if(returnCode == 32766) {
				onRoomNotUnique?.Invoke();
            }
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
