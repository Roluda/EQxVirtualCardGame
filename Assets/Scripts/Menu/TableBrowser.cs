using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;

namespace EQx.Menu {
    public class TableBrowser : MonoBehaviourPunCallbacks {
		[SerializeField]
		string gameVersion = "Develop";

		[SerializeField]
		Transform content = null;
		[SerializeField]
		GameObject serverOption = null;
		[SerializeField]
		TMP_Text log = default;

		[SerializeField]
		float reconnectInterval = 10;

		public Action onRoomNotUnique;

		static string userID = "";

        public void ConnectToPhoton() {
            if (!PhotonNetwork.IsConnected) {
				log.text = "Connecting to Master Server...";
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

        public override void OnErrorInfo(ErrorInfo errorInfo) {
			log.text= "Network error: " + errorInfo.Info;
        }


        float timer = 0;
        private void Update() {
			timer += Time.deltaTime;
            if (timer > reconnectInterval) {
				ConnectToPhoton();
			}
        }

        public override void OnConnectedToMaster() {
			userID = PhotonNetwork.LocalPlayer.UserId;
			log.text = "Connected to Master Server";
			Debug.Log("ConnectedToMaster");
			PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby() {
			log.text = "Connected to Multiplayer Lobby";
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
			log.text = "Creating Room...";
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
			log.text = message;
			Debug.LogWarning(message);
        }

        public override void OnJoinRoomFailed(short returnCode, string message) {
			log.text = message;
			Debug.LogWarning(message);
        }

        private void OnApplicationQuit() {
			if (PhotonNetwork.IsConnected) {
				PhotonNetwork.Disconnect();
			}
        }
    }
}
