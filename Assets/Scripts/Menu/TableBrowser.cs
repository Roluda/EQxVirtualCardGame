﻿using BeardedManStudios.SimpleJSON;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;

namespace EQx.Menu {
    public class TableBrowser : MonoBehaviour {
		[SerializeField]
		string masterServerHost = "127.0.0.1";
		[SerializeField]
		ushort masterServerPort = 15940;

		[SerializeField, Range(0, 180)]
		float refreshInterval = 10;

		[SerializeField]
		string gameName = "EQx CG";
		[SerializeField]
		string gameType = "any";
		[SerializeField]
		string gameMode = "all";

		[SerializeField]
		string hostGamePrefix = "Table of ";
		[SerializeField]
		string hostDefaultName = "Anonymous";

		[SerializeField]
		Transform content = null;
		[SerializeField]
		GameObject serverOption = null;
		[SerializeField]
		GameObject networkManagerPrefab = null;

		[SerializeField]
		bool useMainThreadManagerForRPCs = true;


		TCPClient client = null;
		NetworkManager networkManager = null;
		NetWorker server = null;
		float timer = 0;

        private void Awake() {
			MainThreadManager.Create();
		}

		private void Start() {
			if (useMainThreadManagerForRPCs)
				Rpc.MainThreadRunner = MainThreadManager.Instance;
			Refresh();
		}

        private void Update() {
			timer += Time.deltaTime;
			if (timer > refreshInterval) {
				timer = 0;
				Refresh();
            }
        }

        public void CreateServerOption(string name, UnityEngine.Events.UnityAction callback) {
			MainThreadManager.Run(() => {
				var option = Instantiate(serverOption);
				option.transform.SetParent(content);
				var browserItem = option.GetComponent<TableOption>();
				if (browserItem != null)
					browserItem.SetData(name, callback);
			});
		}

		public void Refresh() {
			// Clear out all the currently listed servers
			for (int i = content.childCount - 1; i >= 0; --i)
				Destroy(content.GetChild(i).gameObject);

			// The Master Server communicates over TCP
			client = new TCPMasterClient();

			// Once this client has been accepted by the master server it should sent it's get request
			client.serverAccepted += (sender) => {
				try {
					// Create the get request with the desired filters
					JSONNode sendData = JSONNode.Parse("{}");
					JSONClass getData = new JSONClass();
					getData.Add("id", gameName);
					getData.Add("type", gameType);
					getData.Add("mode", gameMode);

					sendData.Add("get", getData);

					// Send the request to the server
					client.Send(BeardedManStudios.Forge.Networking.Frame.Text.CreateFromString(client.Time.Timestep, sendData.ToString(), true, Receivers.Server, MessageGroupIds.MASTER_SERVER_GET, true));
				} catch {
					// If anything fails, then this client needs to be disconnected
					client.Disconnect(true);
					client = null;
				}
			};

			// An event that is raised when the server responds with hosts
			client.textMessageReceived += (player, frame, sender) => {
				try {
					// Get the list of hosts to iterate through from the frame payload
					JSONNode data = JSONNode.Parse(frame.ToString());
					if (data["hosts"] != null) {
						MasterServerResponse response = new MasterServerResponse(data["hosts"].AsArray);

						if (response != null && response.serverResponse.Count > 0) {
							// Go through all of the available hosts and add them to the server browser
							foreach (MasterServerResponse.Server server in response.serverResponse) {
								string protocol = server.Protocol;
								string address = server.Address;
								ushort port = server.Port;
								string name = server.Name;

								// name, address, port, comment, type, mode, players, maxPlayers, protocol
								CreateServerOption(name, () => {
									// Determine which protocol should be used when this client connects
									NetWorker socket = null;

									if (protocol == "tcp") {
										socket = new TCPClient();
										Debug.Log("Trying to connect to :" + address + " on Port: " + port);
										((TCPClient)socket).Connect(address, port);
										Debug.Log("Connected?");
									}

									if (socket == null)
										throw new Exception("No socket of type " + protocol + " could be established");

									Connected(socket);
								});
							}
						}
					}
				} finally {
					if (client != null) {
						// If we succeed or fail the client needs to disconnect from the Master Server
						client.Disconnect(true);
						client = null;
					}
				}
			};

			client.Connect(masterServerHost, (ushort)masterServerPort);
		}

		public void Host() {
			server = new TCPServer(6);
			((TCPServer)server).Connect();

			server.playerTimeout += (player, sender) =>
			{
				Debug.Log("Player " + player.NetworkId + " timed out");
			};

			Connected(server);
		}

		/// <summary>
		/// called when connected to server as serevr or client
		/// </summary>
		/// <param name="networker"></param>
		public void Connected(NetWorker networker) {
			if (!networker.IsBound) {
				Debug.LogError("NetWorker failed to bind");
				return;
            } else {
				Debug.Log("NetWorker bound");
            }

			if (networkManager == null && networkManagerPrefab == null) {
				Debug.LogWarning("A network manager was not provided, generating a new one instead");
				networkManagerPrefab = new GameObject("Network Manager");
				networkManager = networkManagerPrefab.AddComponent<NetworkManager>();
			} else if (networkManager == null)
				networkManager = Instantiate(networkManagerPrefab).GetComponent<NetworkManager>();

			// If we are using the master server we need to get the registration data
			string serverId = gameName;
			string serverName = hostGamePrefix + PlayerPrefs.GetString(PlayerPrefKeys.PLAYERNAME, hostDefaultName);
			string type = "Standard";
			string mode = "Teams";
			string comment = "Join me";

			JSONNode masterServerData = networkManager.MasterServerRegisterData(networker, serverId, serverName, type, mode, comment);

			networkManager.Initialize(networker, masterServerHost, masterServerPort, masterServerData);
			//NetworkObject.Flush(networker);

			SceneManager.sceneLoaded += (s, l) => NetworkManager.Instance.SceneReady(s, l);
			//SceneManager.sceneLoaded += (scene, loadmode) => NetworkObject.Flush(networker);
			SceneManager.sceneLoaded += (a, b) => Debug.Log("SceneLoaded and Flushed");

			if (networker is IServer) {
				Debug.Log("Hosting Table");
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
			} else {
				Debug.Log("Joinig Table as Client");
			}
		}

		public void SetMasterServerIP(string ip) {
			masterServerHost = ip;
        }

		private void OnApplicationQuit() {
			if (server != null)
				server.Disconnect(true);
		}
	}
}
