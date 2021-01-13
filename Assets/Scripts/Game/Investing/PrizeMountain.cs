using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Investing {
    public class PrizeMountain : MonoBehaviour{

        [SerializeField]
        PileMountain mountain;
        // Start is called before the first frame update
        void Start() {
            InvestmentManager.instance.onPrizeUpdated += CalculateMountain;
        }

        void CalculateMountain() {
            Debug.Log(name + ".CalculateMountain");
            mountain.capital = InvestmentManager.instance.prize;
        }
    }
}
