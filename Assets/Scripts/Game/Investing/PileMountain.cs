using EQx.Game.Investing;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EQx.Game.Investing {
    public class PileMountain : MonoBehaviour {

        [SerializeField]
        Transform pileBase = default;
        [SerializeField]
        CoinPile pilePrefab = default;
        [SerializeField]
        int columns = 3;
        [SerializeField]
        int rows = 2;
        [SerializeField]
        float pileDistance = 0.02f;
        int maxPiles => columns * rows;

        [SerializeField]
        List<CoinPile> coinPiles = default;

        public int coins => coinPiles.Sum(pile => pile.count);

        int maxCoins => coinPiles.Sum(pile => pile.maxCoins);
        public int capital {
            get => coinPiles.Sum(pile => pile.amount);
            set {
                value = Mathf.Clamp(value, 0, maxCoins);
                if (value > capital) {
                    int diff = value - capital;
                    while (diff > 0) {
                        diff = coinPiles.Where(pile => pile.amount < pile.maxCoins).First().AddCoins(diff);
                        
                    }
                } else if (value < capital) {
                    int diff = value - capital;
                    while (diff < 0) {
                        diff = coinPiles.Where(pile => pile.amount > 0).Last().AddCoins(diff);
                    }
                }
            }
        }

        public bool highlighted => coinPiles.Any(pile => pile.highlighted);

        private void Start() {
            BuildPiles();
        }

        private void BuildPiles() {
            Debug.Log(name + ".BuildPiles");
            coinPiles = new List<CoinPile>();
            for (int c = 0; c<columns; c++) {
                for(int r = 0;r<rows; r++) {
                    float distance = pilePrefab.maxRadius * 2 + pileDistance;
                    float column = c * distance + (distance / 2) * r % 2;
                    float row = - r * distance / Mathf.Cos(60);
                    Vector3 localPosition = new Vector3(column, 0, row);
                    coinPiles.Add(Instantiate(pilePrefab, pileBase.position + localPosition, pileBase.rotation, pileBase));
                }
            }
        }
    }
}
