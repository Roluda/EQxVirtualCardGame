using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TableOption : MonoBehaviour {
    public TMP_Text nameText;
    public Button connectutton;

    public void SetData(string name, UnityAction callback) {
        nameText.text = name;
        connectutton.onClick.AddListener(callback);
    }

}
