using EQx.Game;
using EQx.Game.CountryCards;
using EQx.Game.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace EQx.Analytics {
    public class RoundTracking : MonoBehaviour {
        [SerializeField]
        string winnerEvent = "winner";

        // Start is called before the first frame update
        void Start() {
#if !UNITY_EDITOR
            RoundManager.instance.onBettingEnded += BettingEndedListener;
#endif
        }


        private void BettingEndedListener() {
            foreach(var stat in RoundManager.instance.playerStats) {
                if (stat.won) {
                    int cardID = stat.placedCard;
                    EQxCountryData country = CountryCardDatabase.instance.GetCountry(cardID);
                    EQxVariableType demand = RoundManager.instance.currentDemand;
                    float baseValue = stat.baseValue;
                    float bonusValue = stat.bonusValue;
                    float combinedValue = stat.combinedValue;
                    int competition = RoundManager.instance.playerStats.Count;
                    AnalyticsEvent.Custom(winnerEvent, new Dictionary<string, object> {
                        {"country_name", country.countryName},
                        {"demand", demand.ToString()},
                        {"country_demand_base_value", baseValue},
                        {"country_demand_bonus_value_from_player", bonusValue},
                        {"country_demand_combined_value", combinedValue},
                        {"competition", competition}
                    });;;
                }
            }
        }
    }
}