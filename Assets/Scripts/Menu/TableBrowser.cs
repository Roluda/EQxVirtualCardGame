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

		private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
		private List<GameObject> serverOptions = new List<GameObject>();

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

        public GameObject CreateServerOption(string name, int currentPlayers, int maxPlayers, UnityEngine.Events.UnityAction callback) {
			var option = Instantiate(serverOption);
			option.transform.SetParent(content);
			var browserItem = option.GetComponent<TableOption>();
			if (browserItem != null)
				browserItem.SetData(name, currentPlayers, maxPlayers, callback);
			return option;
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
			ClearRoomListView();
			UpdateCachedRoomList(roomList);
			UpdateRoomListView();
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

		private void ClearRoomListView() {
			foreach (GameObject entry in serverOptions) {
				Destroy(entry.gameObject);
			}
			serverOptions.Clear();
		}

		private void UpdateCachedRoomList(List<RoomInfo> roomList) {
			foreach (RoomInfo info in roomList) {
				if (!info.IsOpen || !info.IsVisible || info.RemovedFromList) {
					if (cachedRoomList.ContainsKey(info.Name)) {
						cachedRoomList.Remove(info.Name);
					}

					continue;
				}
				if (cachedRoomList.ContainsKey(info.Name)) {
					cachedRoomList[info.Name] = info;
				}
				else {
					cachedRoomList.Add(info.Name, info);
				}
			}
		}

		private void UpdateRoomListView() {
			foreach (RoomInfo room in cachedRoomList.Values) {
				serverOptions.Add(CreateServerOption(room.Name, room.PlayerCount, room.MaxPlayers, () => {
					PhotonNetwork.JoinRoom(room.Name);
				}));
			}
		}
	}
}
