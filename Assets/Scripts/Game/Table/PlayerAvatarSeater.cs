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

        private void PlayerLeftTableListener(CardPlayer player) {
            var removedAvatar = avatars.Where(avatar => avatar.observedPlayer == player).First();
            avatars.Remove(removedAvatar);
            Destroy(removedAvatar);
        }

        private void PlayerSeatedListener(CardPlayer player) {
            var newAvatar = Instantiate(avatarPrefab, transform);
            newAvatar.Initialize(player);
            avatars.Add(newAvatar);
        }

        private void TableUpdatedListener() {
            MapAvatarsToSeats();
        }

        private void MapAvatarsToSeats() {
            for(int i=0; i < avatars.Count; i++) {
                avatars[i].transform.position = seats[i].position;
            }
        }

        private void OnDisable() {
            GameTable.instance.onTableUpdated -= TableUpdatedListener;
            GameTable.instance.onPlayerSeated -= PlayerSeatedListener;
            GameTable.instance.onPlayerLeftTable -= PlayerLeftTableListener;
        }

        // Start is called before the first frame update
        void Start() {
            GameTable.instance.onTableUpdated += TableUpdatedListener;
            GameTable.instance.onPlayerSeated += PlayerSeatedListener;
            GameTable.instance.onPlayerLeftTable += PlayerLeftTableListener;
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
