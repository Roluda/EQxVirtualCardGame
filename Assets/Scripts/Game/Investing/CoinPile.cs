using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using EQx.Game.Audio;

namespace EQx.Game.Investing {
    public class CoinPile : MonoBehaviour {

        [SerializeField]
        Coin coinPrefab = default;
        [SerializeField, Range(0, 1)]
        float axisShift = .1f;
        [SerializeField, Range(0, 90), Tooltip("maximum Angle in Degree")]
        float rotationShift = 20;
        [SerializeField, Range(0, 30)]
        float spawnFrequency = 2;
        [SerializeField, Range(0,100)]
        public int maxCoins = 10;
        [SerializeField, Range(0,20)]
        int spawnHeight = 1;
        [SerializeField]
        TMP_Text countDisplay = default;
        [SerializeField]
        string displayPrefix = "";
        [SerializeField]
        float displayHeight = 0.1f;
        [SerializeField]
        RandomSFX coinSFX = default;

        [Header("Debug")]
        [SerializeField]
        int spawnCoins = 0;

        public bool highlighted => pile.Any(coin => coin.highlighted || coin.dragging);

        public float maxRadius => coinPrefab.radius + coinPrefab.radius * axisShift;
        public int count => pile.Where(coin => coin != null).Count();

        Queue<Coin> pile = new Queue<Coin>();
        float timer = 0;

        private void OnValidate() {
            if (spawnCoins>0) {
                AddCoins(spawnCoins);
                spawnCoins = 0;
            }
        }

        int targetAmount = 0;

        public void SetAmount(int amount) {
            targetAmount = Mathf.Clamp(amount, 0, maxCoins);
        }

        public int amount => targetAmount;

        /// <summary>
        /// returns the exchange if the pile is full, debt when pile is empty
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int AddCoins(int amount) {
            Debug.Log(name + ".AddCoins: " + amount);
            int oldAmount = targetAmount;
            SetAmount(targetAmount + amount);
            int newAmount = targetAmount;
            int difference = newAmount - oldAmount;
            return amount - difference;
        }

        public int RemoveCoins(int amount) {
            Debug.Log(name + ".RemoveCoins: " + amount);
            int debt = amount - targetAmount;
            SetAmount(targetAmount - amount);
            return debt;
        }

        // Update is called once per frame
        void Update() {
            timer += Time.deltaTime;
            if (pile.Count>0 && !pile.Peek()) {
                pile.Dequeue();
            }
            if (timer >= 1 / spawnFrequency) {
                timer = 0;
                if (count < targetAmount) {
                    pile.Enqueue(Instantiate(coinPrefab, CalculateSpawnPosition(), CalculateRotation(), transform));
                    coinSFX.Play();
                }else if(count > targetAmount) {
                    Destroy(pile.Dequeue().gameObject);
                    coinSFX.Play();
                }
            }
            UpdateCountDisplay();
        }

        void UpdateCountDisplay() {
            if (!countDisplay) {
                return;
            } 
            countDisplay.gameObject.SetActive(highlighted);
            countDisplay.text = displayPrefix + count.ToString();
            countDisplay.transform.position = transform.position + Vector3.up * (count + spawnHeight) * coinPrefab.height + Vector3.up * displayHeight;
            countDisplay.transform.rotation = Quaternion.identity;
        }

        private Quaternion CalculateRotation() {
            return Quaternion.Euler(new Vector3(UnityEngine.Random.Range(-rotationShift, rotationShift), 0, UnityEngine.Random.Range(-rotationShift, rotationShift)));
        }

        private Vector3 CalculateSpawnPosition() {
            var offset = UnityEngine.Random.insideUnitCircle * coinPrefab.radius * axisShift;
            return transform.position + Vector3.up * (spawnHeight+count) * coinPrefab.height + new Vector3(offset.x, 0, offset.y);
        }
    }
}
