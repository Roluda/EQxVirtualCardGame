using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EQx.Game.Investing {
    public class PrizeMountain : MonoBehaviour{

        [SerializeField]
        PileMountain mountain;

        [SerializeField]
        TMP_Text growthInfo = default;

        [SerializeField]
        float growthInfoDuration = 2;

        // Start is called before the first frame update
        void Start() {
            InvestmentManager.instance.onPrizeUpdated += CalculateMountain;
            InvestmentManager.instance.onEconomyGrowth += EconomyGrowthListener;
            growthInfo.gameObject.SetActive(false);
        }

        private void EconomyGrowthListener(int growth) {
            int increase = (int)(InvestmentManager.instance.economicGrowth * 100) - 100;
            growthInfo.text = $"+{increase}% ({growth})";
            StartCoroutine(DisplayGrowthInfo());
        }

        IEnumerator DisplayGrowthInfo() {
            growthInfo.gameObject.SetActive(true);
            yield return new WaitForSeconds(growthInfoDuration);
            growthInfo.gameObject.SetActive(false);
        }

        void CalculateMountain() {
            Debug.Log(name + ".CalculateMountain");
            mountain.capital = InvestmentManager.instance.prize;
        }
    }
}
