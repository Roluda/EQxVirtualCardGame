using EQx.Game.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace EQx.Game.Table {
    public class PlayerAvatarSeater : MonoBehaviour {
        [SerializeField]
        PlayerAvatar avatarPrefab = default;

        [SerializeField]
        List<Transform> seats = default;

        List<PlayerAvatar> avatars = new List<PlayerAvatar>();

        private void PlayerUnregisteredListener(CardPlayer player) {
            if (player == CardPlayer.localPlayer) {
                return;
            }
            Debug.Log(name + "PlayerLeftTableListener");
            var removedAvatar = avatars.Where(avatar => avatar.observedPlayer == player).First();
            avatars.Remove(removedAvatar);
            Destroy(removedAvatar.gameObject);
        }

        private void PlayerRegisteredListener(CardPlayer player) {
            if (player == CardPlayer.localPlayer) {
                return;
            }
            Debug.Log(name + "PlayerSeatedListener");
            var newAvatar = Instantiate(avatarPrefab, transform);
            newAvatar.Initialize(player);
            avatars.Add(newAvatar);
        }

        private void TableUpdatedListener() {
            Debug.Log(name + "TableUpdatedListener");
            MapAvatarsToSeats();
        }

        private void MapAvatarsToSeats() {
            int mySeatNumber = CardPlayer.localPlayer.seatNumber;
            avatars.Sort((a, b) => a.observedPlayer.seatNumber.CompareTo(b.observedPlayer.seatNumber));
            for(int i=0; i < avatars.Count; i++) {
                int offset = avatars[i].observedPlayer.seatNumber - mySeatNumber;
                if (offset < 0) {
                    offset = seats.Count + offset;
                } else {
                    offset--;
                }
                avatars[i].transform.position = seats[offset].position;
                avatars[i].transform.rotation = seats[offset].rotation;
            }
        }

        // Start is called before the first frame update
        void Start() {
            RoundManager.instance.onRegisterUpdate += TableUpdatedListener;
            RoundManager.instance.onPlayerRegister += PlayerRegisteredListener;
            RoundManager.instance.onPlayerUnregister += PlayerUnregisteredListener;
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
