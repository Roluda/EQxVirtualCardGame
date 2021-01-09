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
        List<CoinPile> capitalPiles = default;

        int maxCapital => capitalPiles.Sum(pile => pile.maxCoins);
        public int capital {
            get => capitalPiles.Sum(pile => pile.targetAmount);
            set {
                value = Mathf.Clamp(value, 0, maxCapital);

                int i = 0;
                if (value > capital) {
                    int diff = value - capital;
                    while (diff > 0) {
                        diff = capitalPiles.Where(pile => pile.targetAmount < pile.maxCoins).First().AddCoins(diff);
                        
                    }
                } else if (value < capital) {
                    int diff = capital - value;
                    while (diff > 0) {
                        diff = capitalPiles.Where(pile => pile.targetAmount > 0).First().RemoveCoins(diff);
                    }
                }
            }
        }

        [SerializeField]
        bool gainTenCoins = false;

        private void OnValidate() {
            if (gainTenCoins) {
                gainTenCoins = false;
                capital += 10;
            }
        }

        private void Start() {
            BuildPiles();
        }

        private void BuildPiles() {
            Debug.Log(name + ".BuildPiles");
            capitalPiles = new List<CoinPile>();
            for (int c = 0; c<columns; c++) {
                for(int r = 0;r<rows; r++) {
                    float distance = pilePrefab.maxRadius * 2 + pileDistance;
                    float column = c * distance + (distance / 2) * r % 2;
                    float row = - r * distance / Mathf.Cos(60);
                    Vector3 localPosition = new Vector3(column, 0, row);
                    capitalPiles.Add(Instantiate(pilePrefab, pileBase.position + localPosition, pileBase.rotation, pileBase));
                }
            }
        }
    }
}
