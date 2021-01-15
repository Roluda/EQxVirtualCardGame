using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EQx.Menu {
    public class URLRequest : MonoBehaviour {
        [SerializeField]
        string url = "https://elitequality.org/#HomeWhy";

        public void OpenURl() {
            Application.OpenURL(url);
        }
    }
}
