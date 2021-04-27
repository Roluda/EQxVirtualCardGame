using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Game.Table {
    [CreateAssetMenu(fileName="Round_New", menuName = "Round")]
    public class RoundData : ScriptableObject {
        [SerializeField]
        List<EQxVariableType> possibleDemands = default;

        public EQxVariableType randomDemand {
            get {
                if(possibleDemands.Count >0) {
                    return possibleDemands[Random.Range(0, possibleDemands.Count)];
                } else {
                    return EQxVariableType.ValueCreation;
                }
            }
        }
    }
}
