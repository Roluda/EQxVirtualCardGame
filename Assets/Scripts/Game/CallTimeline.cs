using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EQx {
    public class CallTimeline : MonoBehaviour {

        [Serializable]
        public class TimelineAction : UnityEvent { };

        [Serializable]
        public class Keyframe {
            [SerializeField, Range(0, 60)]
            public float start = 0;
            [SerializeField]
            public TimelineAction action;
            [HideInInspector]
            public bool wasPlayed;
        }
        [SerializeField]
        bool sort = false;

        [SerializeField]
        List<Keyframe> actions = default;

        private void OnValidate() {
            if (sort) {
                sort = false;
                actions.Sort((x, y) => x.start.CompareTo(y.start));
            }
        }

        bool playing = false;
        [SerializeField]
        float timer = 0;

        public void Play() {
            playing = true;
        }

        public void Pause() {
            playing = false;
        }

        public void Stop() {
            playing = false;
            timer = 0;
            foreach(var action in actions) {
                action.wasPlayed = false;
            }
        }

        float lastUpdateTime = 0;

        // Update is called once per frame
        void Update() {
            float realDeltaTime = Time.realtimeSinceStartup - lastUpdateTime;
            if (playing) {
                timer += realDeltaTime;
                foreach (var action in actions) {
                    if (timer >= action.start && !action.wasPlayed) {
                        action.action?.Invoke();
                        action.wasPlayed = true;
                    }
                }
            }
            lastUpdateTime = Time.realtimeSinceStartup;
        }
    }
}
