using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EQx.Game {
    public class EQxVariableDatabase : MonoBehaviour {

        public static EQxVariableDatabase instance = null;

        [SerializeField]
        public List<EQxVariableData> data = default;

        // Start is called before the first frame update
        private void Awake() {
            if(instance != null) {
                Destroy(gameObject);
            } else {
                instance = this;
                DontDestroyOnLoad(this);
            }
        }

        public EQxVariableData GetVariable(EQxVariableType type) {
            return data.Where(variable => variable.type == type).FirstOrDefault();
        }

        public EQxVariableData GetVariable(string name) {
            return data.Where(variable => variable.variableName == name).FirstOrDefault();
        }
    }
}
