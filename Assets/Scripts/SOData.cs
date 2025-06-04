using UnityEngine;
using UnityEngine.UI;

public class SOData : MonoBehaviour {
    [SerializeField] private PlayerModelSO playerModel;
    [SerializeField] private Button testeButton;

    private void Start() {
        testeButton.onClick.AddListener(OnTesteButtonClick);
    }

    private void OnTesteButtonClick() {
        if (playerModel != null && playerModel.Value != null) {
            Debug.Log("Player Nickname: " + playerModel.Value.User_Nickname);
            Debug.Log("Player ID: " + playerModel.Value.User_Id);
        } else {
            Debug.LogWarning("PlayerModelSO is not assigned.");
        }
    }

}
