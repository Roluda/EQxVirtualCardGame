using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.SimpleJSON;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerMenu : MonoBehaviour
{
	public bool dontChangeSceneOnConnect = false;
	public string masterServerHost = string.Empty;
	public ushort masterServerPort = 15940;

	public GameObject networkManagerPrefab = null;
	public GameObject[] toggledButtons;
	private NetworkManager networkManager = null;
	private NetWorker server;

	private List<Button> uiButtons = new List<Button>();

	private void LocalServerLocated(NetWorker.BroadcastEndpoints endpoint, NetWorker sender)
	{
		Debug.Log("Found endpoint: " + endpoint.Address + ":" + endpoint.Port);
	}

	public void Connected(NetWorker networker)
	{


		if (networker is IServer)
		{
			if (!dontChangeSceneOnConnect)
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
			else
				NetworkObject.Flush(networker); //Called because we are already in the correct scene!
		}
	}
}
