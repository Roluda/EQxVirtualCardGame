using EQx.Game.Audio;
using UnityEngine;

namespace EQx.Game.CountryCards {
    public class CardAudio : CountryCardComponent {
        [SerializeField]
        RandomSFX cardSFX = default;


        protected override void CardPlayedListener() {
            AudioManager.instance.PlayOneShot(cardSFX);
        }

        protected override void CardSelectedListener(CountryCard card) {
            AudioManager.instance.PlayOneShot(cardSFX);
        }

        protected override void CardDrawnListener() {
            AudioManager.instance.PlayOneShot(cardSFX);
        }
    }
}
