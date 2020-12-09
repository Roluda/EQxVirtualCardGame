using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedRPC("{\"types\":[[\"string\"][\"int\"][][][\"int\"][][]]")]
	[GeneratedRPCVariableNames("{\"types\":[[\"name\"][\"id\"][][][\"id\"][][]]")]
	public abstract partial class CardPlayerBehavior : NetworkBehavior
	{
		public const byte RPC_SET_NAME = 0 + 5;
		public const byte RPC_PLACE_CARD = 1 + 5;
		public const byte RPC_END_TURN = 2 + 5;
		public const byte RPC_START_TURN = 3 + 5;
		public const byte RPC_RECEIVE_CARD = 4 + 5;
		public const byte RPC_REQUEST_CARD = 5 + 5;
		public const byte RPC_WIN_ROUND = 6 + 5;
		
		public CardPlayerNetworkObject networkObject = null;

		public override void Initialize(NetworkObject obj)
		{
			// We have already initialized this object
			if (networkObject != null && networkObject.AttachedBehavior != null)
				return;
			
			networkObject = (CardPlayerNetworkObject)obj;
			networkObject.AttachedBehavior = this;

			base.SetupHelperRpcs(networkObject);
			networkObject.RegisterRpc("SetName", SetName, typeof(string));
			networkObject.RegisterRpc("PlaceCard", PlaceCard, typeof(int));
			networkObject.RegisterRpc("EndTurn", EndTurn);
			networkObject.RegisterRpc("StartTurn", StartTurn);
			networkObject.RegisterRpc("ReceiveCard", ReceiveCard, typeof(int));
			networkObject.RegisterRpc("RequestCard", RequestCard);
			networkObject.RegisterRpc("WinRound", WinRound);

			networkObject.onDestroy += DestroyGameObject;

			if (!obj.IsOwner)
			{
				if (!skipAttachIds.ContainsKey(obj.NetworkId)){
					uint newId = obj.NetworkId + 1;
					ProcessOthers(gameObject.transform, ref newId);
				}
				else
					skipAttachIds.Remove(obj.NetworkId);
			}

			if (obj.Metadata != null)
			{
				byte transformFlags = obj.Metadata[0];

				if (transformFlags != 0)
				{
					BMSByte metadataTransform = new BMSByte();
					metadataTransform.Clone(obj.Metadata);
					metadataTransform.MoveStartIndex(1);

					if ((transformFlags & 0x01) != 0 && (transformFlags & 0x02) != 0)
					{
						MainThreadManager.Run(() =>
						{
							transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform);
							transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform);
						});
					}
					else if ((transformFlags & 0x01) != 0)
					{
						MainThreadManager.Run(() => { transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform); });
					}
					else if ((transformFlags & 0x02) != 0)
					{
						MainThreadManager.Run(() => { transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform); });
					}
				}
			}

			MainThreadManager.Run(() =>
			{
				NetworkStart();
				networkObject.Networker.FlushCreateActions(networkObject);
			});
		}

		protected override void CompleteRegistration()
		{
			base.CompleteRegistration();
			networkObject.ReleaseCreateBuffer();
		}

		public override void Initialize(NetWorker networker, byte[] metadata = null)
		{
			Initialize(new CardPlayerNetworkObject(networker, createCode: TempAttachCode, metadata: metadata));
		}

		private void DestroyGameObject(NetWorker sender)
		{
			MainThreadManager.Run(() => { try { Destroy(gameObject); } catch { } });
			networkObject.onDestroy -= DestroyGameObject;
		}

		public override NetworkObject CreateNetworkObject(NetWorker networker, int createCode, byte[] metadata = null)
		{
			return new CardPlayerNetworkObject(networker, this, createCode, metadata);
		}

		protected override void InitializedTransform()
		{
			networkObject.SnapInterpolations();
		}

		/// <summary>
		/// Arguments:
		/// string name
		/// </summary>
		public abstract void SetName(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// int id
		/// </summary>
		public abstract void PlaceCard(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void EndTurn(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void StartTurn(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void ReceiveCard(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void RequestCard(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// </summary>
		public abstract void WinRound(RpcArgs args);

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}