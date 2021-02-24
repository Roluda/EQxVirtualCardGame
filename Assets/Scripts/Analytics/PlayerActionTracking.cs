using EQx.Game;
using EQx.Game.CountryCards;
using EQx.Game.Investing;
using EQx.Game.Player;
using EQx.Game.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace EQx.Analytics {
    public class PlayerActionTracking : MonoBehaviour {
        [SerializeField]
        string commitmentEventName = "Commitment";
        [SerializeField]
        string placingEventName = "Card Placed";

        private void Awake() {
            CardPlayer.localPlayerReady += Initialize;
        }

        private void Initialize(CardPlayer player) {
            CardPlayer.localPlayerReady -= Initialize;
#if !UNITY_EDITOR
            player.onPlacedCard += CardPlacedListener;
            player.onInvestedCoins += CoinsInvestedListener;
#endif
        }

        private void CoinsInvestedListener(CardPlayer player, int coins) {
            int card = player.placedCardID;
            EQxCountryData country = CountryCardDatabase.instance.GetCountry(card);
            int playerCommitment = coins - InvestmentManager.instance.blind;
            EQxVariableType demand = RoundManager.instance.currentDemand;
            AnalyticsEvent.Custom(commitmentEventName, new Dictionary<string, object> {
                {"amount_commited", playerCommitment },
                {"country", country.countryName.ToString()},
                {"demand",  demand},
                {"country_demand_value", country.GetValue(demand)}
            });
        }

        private void CardPlacedListener(CardPlayer player, int id) {
            EQxCountryData country = CountryCardDatabase.instance.GetCountry(id);
            EQxVariableType demand = RoundManager.instance.currentDemand;
            AnalyticsEvent.Custom(placingEventName, new Dictionary<string, object> {
                {"country_placed", country.countryName.ToString()},
                {"demand",  demand},
                {"country_demand_value", country.GetValue(demand)}
            });
        }
    }
}
