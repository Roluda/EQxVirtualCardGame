using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

namespace EQx.Game.Table {
    public class TableSeater : MonoBehaviour {
        [SerializeField]
        float spawnRadius = 4;
        [SerializeField]
        float spawnHeight = 1;
        // Start is called before the first frame update
        void Start() {
            Vector2 spawnPoint = Random.insideUnitCircle * spawnRadius;
            NetworkManager.Instance.InstantiateTestPlayer(0, new Vector3(spawnPoint.x, spawnHeight, spawnPoint.y), Quaternion.identity);
            Debug.Log("Seated new player");
        }

        // Update is called once per frame
        void Update() {

        }
    }
}
