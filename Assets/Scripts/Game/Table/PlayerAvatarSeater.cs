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
        List<Seat> seats = default;

        List<PlayerAvatar> avatars = new List<PlayerAvatar>();

        public void RemoveAllPlacedCards() {
            foreach(var avatar in avatars) {
                avatar.RemovePlacedCard();
            }
        }

        private void PlayerUnregisteredListener(CardPlayer player) {
            if (player == CardPlayer.localPlayer) {
                return;
            }
            var removedAvatar = avatars.Where(avatar => avatar.observedPlayer == player).First();
            avatars.Remove(removedAvatar);
            Destroy(removedAvatar.gameObject);
        }

        private void PlayerRegisteredListener(CardPlayer player) {
            if (player == CardPlayer.localPlayer) {
                return;
            }
            var newAvatar = Instantiate(avatarPrefab, transform);
            newAvatar.Initialize(player);
            avatars.Add(newAvatar);
        }

        private void MapAvatarsToSeats() {
            if (!CardPlayer.localPlayer) {
                return;
            }
            int mySeatNumber = RoundManager.instance.GetParticipant(CardPlayer.localPlayer).seatNumber;
            for(int i=0; i < avatars.Count; i++) {
                int seat = RoundManager.instance.GetParticipant(avatars[i].observedPlayer).seatNumber;
                int offset = seat - mySeatNumber;
                if (offset < 0) {
                    offset = seats.Count + offset;
                } else {
                    offset--;
                }
                avatars[i].TakeSeat(seats[offset]);
            }
        }

        // Start is called before the first frame update
        void Start() {
            RoundManager.instance.onRegisterUpdate += MapAvatarsToSeats;
            RoundManager.instance.onPlayerRegister += PlayerRegisteredListener;
            RoundManager.instance.onPlayerUnregister += PlayerUnregisteredListener;
        }
    }
}
