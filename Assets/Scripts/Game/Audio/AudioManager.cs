using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Audio {
    public class AudioManager : MonoBehaviour {

        public static AudioManager instance = null;

        [SerializeField]
        AudioSource sfxPlayer = null;

        // Start is called before the first frame update
        void Awake() {
            if(instance != null) {
                Destroy(gameObject);
            } else {
                instance = this;
            }
        }

        public void PlayOneShot(AudioClip clip) {
            sfxPlayer.PlayOneShot(clip);
        }

        public void PlayOneShot(RandomSFX randomSFX) {
            sfxPlayer.PlayOneShot(randomSFX.next, randomSFX.volume);
        }

        private void OnDestroy() {
            if(instance == this) {
                instance = null;
            }
        }
    }
}
