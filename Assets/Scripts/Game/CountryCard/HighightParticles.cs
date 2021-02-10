using EQx.Game.CountryCards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighightParticles : CountryCardComponent
{
    [SerializeField]
    ParticleSystem system = default;

    protected override void CardAffordableListener(CountryCard card) {
        system.Play();
    }

    protected override void CardUnaffordableListener(CountryCard card) {
        Debug.Log("Stopping Affordance");
        system.Stop();
    }
}
