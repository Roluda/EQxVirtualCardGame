using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EQx.Game.Investing {
    public class PrizeMountain : MonoBehaviour{

        [SerializeField]
        PileMountain prizeMountain;
        [SerializeField]
        PileMountain debtMountain;

        [SerializeField]
        TMP_Text growthInfo = default;

        [SerializeField]
        TMP_Text infoText = default;
        [SerializeField]
        string infoPrefix = "Prize: ";

        [SerializeField]
        float growthInfoDuration = 2;
        [SerializeField]
        string grwothPrefix = "Economic Value Created:";

        // Start is called before the first frame update
        void Start() {
            InvestmentManager.instance.onPrizeUpdated += CalculateMountain;
            InvestmentManager.instance.onEconomyGrowth += EconomyGrowthListener;
            growthInfo.gameObject.SetActive(false);
        }
        void Update() {
            infoText.text = $"{infoPrefix}{InvestmentManager.instance.prize}";
            infoText.gameObject.SetActive(prizeMountain.highlighted || debtMountain.highlighted);
        }

        private void EconomyGrowthListener(int growth) {
            int increase = (int)(InvestmentManager.instance.economicGrowth * 100) - 100;
            if (growth > 0) {
                growthInfo.text = $"{grwothPrefix} {growth}";
                StartCoroutine(DisplayGrowthInfo());
            }
        }

        IEnumerator DisplayGrowthInfo() {
            growthInfo.gameObject.SetActive(true);
            yield return new WaitForSeconds(growthInfoDuration);
            growthInfo.gameObject.SetActive(false);
        }

        void CalculateMountain() {
            Debug.Log(name + ".CalculateMountain");
            int prize = InvestmentManager.instance.prize;
            if (prize > 0) {
                prizeMountain.capital = prize;
                debtMountain.capital = 0;
            } else {
                prizeMountain.capital = 0;
                debtMountain.capital = -prize;
            }
        }
    }
}
