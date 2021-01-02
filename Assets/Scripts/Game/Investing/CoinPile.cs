using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        int maxCoins = 10;
        [SerializeField, Range(0,20)]
        int spawnHeight = 1;

        [Header("Debug")]
        [SerializeField]
        int spawnCoins = 0;
        [SerializeField]
        int removeCoins = 0;

        public int count => pile.Count;

        Queue<Coin> pile = new Queue<Coin>();
        int spawnRequest = 0;
        float timer = 0;

        private void OnValidate() {
            if (spawnCoins>0) {
                AddCoins(spawnCoins);
                spawnCoins = 0;
            }
            if (removeCoins > 0) {
                RemoveCoins(removeCoins);
                removeCoins = 0;
            }
        }


        /// <summary>
        /// returns the exchange if the pile is full
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int AddCoins(int amount) {
            int space = maxCoins - count + spawnRequest;
            spawnRequest += amount > space ? space : amount;
            int exchange = amount > space ? amount - space : 0;
            return exchange;
        }

        public void RemoveCoins(int amount) {
            int current = count;
            for(int i=0; i < (amount > current? current : amount); i++) {
                Destroy(pile.Dequeue().gameObject);
            }
        }

        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            timer += Time.deltaTime;
            if (timer >= 1 / spawnFrequency) {
                timer = 0;
                if(spawnRequest > 0) {
                    spawnRequest--;
                    pile.Enqueue(Instantiate(coinPrefab, CalculateSpawnPosition(), CalculateRotation(), transform));
                }
            }
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
