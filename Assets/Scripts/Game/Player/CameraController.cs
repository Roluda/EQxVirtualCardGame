using EQx.Game.Table;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EQx.Game.Player {
    public class CameraController : MonoBehaviour {
        // Start is called before the first frame update
        void Start() {
            currentTarget = defaultSpot;
        }
        [SerializeField]
        Transform defaultSpot = default;
        [SerializeField]
        Transform closeUpSpot = default;
        [SerializeField, Range(0,1)]
        float shakeIntensity = default;
        [SerializeField, Range(0,1)]
        float shakeDuration = default;

        [SerializeField, Range(0, 5)]
        float smoothTime = 0.5f;
        [SerializeField, Range(0, 100)]
        float maxVelocity = 1;
        [SerializeField, Range(0, 360)]
        float maxTurnSpeed = 20;

        Vector3 currentVelocity;
        public Transform currentTarget;

        public void GoToDefaultSpot() {
            currentTarget = defaultSpot;
        }

        public void GoToCloseUpSpot() {
            currentTarget = closeUpSpot;
        }

        public void ScreenShake() {
            StartCoroutine(ShakeRoutine());
        }

        IEnumerator ShakeRoutine() {
            float timer = 0;
            while (timer < shakeDuration) {
                timer += Time.deltaTime;
                yield return null;
                transform.position = transform.position + Random.insideUnitSphere * shakeIntensity;
            }
        }


        // Update is called once per frame
        void Update() {
            transform.position = Vector3.SmoothDamp(transform.position, currentTarget.position, ref currentVelocity, smoothTime, maxVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, currentTarget.rotation, maxTurnSpeed * Time.deltaTime);
        }
    }
}
