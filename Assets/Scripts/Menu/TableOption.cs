using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TableOption : MonoBehaviour {
    [SerializeField]
    TMP_Text nameText = default;
    [SerializeField]
    TMP_Text playersText = default;
    [SerializeField]
    Button connectutton;

    public void SetData(string name, int currentPlayers, int maxPlayers, UnityAction callback) {
        playersText.text = $"{currentPlayers}/{maxPlayers}";
        nameText.text = name;
        connectutton.onClick.AddListener(callback);
    }

}
