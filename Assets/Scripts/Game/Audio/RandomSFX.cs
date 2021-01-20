using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Audio {
    [CreateAssetMenu(fileName="RSFX_New", menuName="Audio/RandomSFX",order = 1)]
    public class RandomSFX : ScriptableObject {

        [SerializeField]
        AudioClip[] clips = default;
        [SerializeField]
        public float volume = 1;

        public AudioClip next => clips[Random.Range(0, clips.Length)];
    }
}
