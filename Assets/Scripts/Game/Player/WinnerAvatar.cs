using EQx.Game.CountryCards;
using EQx.Game.Table;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EQx.Game.Player {
    public class WinnerAvatar : MonoBehaviour {
        [SerializeField]
        Image icon = default;
        [SerializeField]
        TMP_Text nameText;
        [SerializeField]
        CountryCard cardPrefab = default;
        [SerializeField]
        RectTransform revealLocation;
        [SerializeField]
        RectTransform cardLocation;
        [SerializeField]
        Vector3 cardScale = Vector3.one;
        [SerializeField]
        float startReturnMargin = 0.05f;

        [SerializeField]
        GameObject winParticlesPrefab = default;

        [HideInInspector]
        public CountryCard cardInstance;
        [HideInInspector]
        public CardPlayer owner;
        [HideInInspector]
        public int id = 2;

        [SerializeField]
        bool spawn = false;
        [SerializeField]
        bool reveal = false;
        [SerializeField]
        bool win = false;
        [SerializeField]
        bool despawn = false;


        private void OnValidate() {
            if (spawn) {
                spawn = false;
                SpawnCard();
            }
            if (reveal) {
                reveal = false;
                Reveal();
            }
            if (win) {
                win = false;
                Win();
            }
            if (despawn) {
                despawn = false;
                if (cardInstance) {
                    Destroy(cardInstance.gameObject);
                }
            }
        }

        public void Initialize(CardPlayer player, int cardID) {
            owner = player;
            nameText.text = player.name;
            id = cardID;
        }

        public void SpawnCard() {
            cardInstance=Instantiate(cardPrefab, cardLocation.transform.position, cardLocation.transform.rotation);
            cardInstance.SetTargetPosition(cardLocation.transform.position);
            cardInstance.SetTargetRotation(cardLocation.transform.rotation.eulerAngles);
            cardInstance.transform.localScale = cardScale;
            cardInstance.data = CountryCardDatabase.instance.GetCountry(id);
            cardInstance.HighlightVariabe(RoundManager.instance.currentDemand);
        }

        public void Reveal() {
            StartCoroutine(RevealRoutine());
        }

        IEnumerator RevealRoutine() {
            cardInstance.SetTargetRotation(revealLocation.transform.rotation.eulerAngles);
            cardInstance.SetTargetPosition(revealLocation.transform.position);
            while(Vector3.Distance(cardInstance.transform.position, revealLocation.transform.position) >= startReturnMargin) {
                yield return null;
            }
            cardInstance.selected = true;
            cardInstance.SetTargetPosition(cardLocation.transform.position);
        }

        public void Win() {
            Instantiate(winParticlesPrefab, cardInstance.transform.position, cardInstance.transform.rotation, cardInstance.transform);
        }

        public void Die() {
            Destroy(gameObject);
            if (cardInstance) {
                Destroy(cardInstance.gameObject);
            }
        }
    }
}
