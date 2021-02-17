using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EQx.Menu {
    public class AvatarSelector : MonoBehaviour {

        [SerializeField]
        Image avatarImage = default;
        [SerializeField]
        string filePath = "Sprites/Characters";

        Sprite[] avatars = default;

        int index;

        // Start is called before the first frame update
        void Start() {
            avatars = Resources.LoadAll<Sprite>(filePath);
            if (PlayerPrefs.HasKey(PlayerPrefKeys.PLAYER_AVATAR) && PlayerPrefs.GetInt(PlayerPrefKeys.PLAYER_AVATAR) < avatars.Length) {
                index = PlayerPrefs.GetInt(PlayerPrefKeys.PLAYER_AVATAR);
            } else {
                index = Random.Range(0, avatars.Length);
            }
            UpdateSprite();
        }

        public void Next() {
            index = (index - 1) < 0 ? avatars.Length - 1 : index - 1;
            UpdateSprite();
        }

        public void Previous() {
            index = (index + 1) % avatars.Length;
            UpdateSprite();
        }

        void UpdateSprite() {
            avatarImage.sprite = avatars[index];
            PlayerPrefs.SetInt(PlayerPrefKeys.PLAYER_AVATAR, index);
        }
    }
}
