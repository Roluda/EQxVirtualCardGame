using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EQx.Game.Chat {
    public class ChatManager : MonoBehaviourPunCallbacks, IPunObservable {
        [SerializeField]
        Transform chatContext = default;
        [SerializeField]
        ChatMessage messagePrefab = default;

        List<ChatMessage> messages = new List<ChatMessage>();

        public void SendChatMessage(string message) {
            if (message.Trim() != "") {
                var sender = PlayerPrefs.GetString(PlayerPrefKeys.PLAYERNAME);
                photonView.RPC("ReceiveChatMessage", RpcTarget.AllViaServer, sender, message);
            }
        }

        [PunRPC]
        void ReceiveChatMessage(string sender, string message) {
            var messageObject = Instantiate(messagePrefab, chatContext);
            messageObject.SetData(sender, message);
            messages.Add(messageObject);
            DisplayAll();
        }

        public void DisplayAll() {
            foreach (var mes in messages) {
                mes.ResetDuration();
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        }
    }
}
