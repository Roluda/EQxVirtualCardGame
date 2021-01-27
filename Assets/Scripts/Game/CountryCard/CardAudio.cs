using EQx.Game.Audio;
using UnityEngine;

namespace EQx.Game.CountryCards {
    public class CardAudio : CountryCardComponent {
        [SerializeField]
        RandomSFX cardSFX = default;


        protected override void CardPlayedListener() {
            cardSFX.Play();
        }

        protected override void CardSelectedListener(CountryCard card) {
            cardSFX.Play();
        }

        protected override void CardDrawnListener() {
            cardSFX.Play();
        }
    }
}
