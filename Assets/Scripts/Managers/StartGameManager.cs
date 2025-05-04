using UnityEngine;
using UnityEngine.UI;

public class StartGameManager : MonoBehaviour
{
    [SerializeField] private Button startGameButton;

    private void Start() {
        startGameButton.onClick.AddListener(OnStartGameButtonClicked);
    }

    private void OnStartGameButtonClicked() {
        Debug.Log("OnStartGameButtonClicked!");

        BoatDraggerManager.Instance.SetDraggingBoatActive(false);

        if (GameManager.Instance.GetLocalPlayerType() == GameManager.PlayerType.Player1) {
            GameManager.Instance.SetIsPlayer1ReadyRpc(true);
        } else {
            GameManager.Instance.SetIsPlayer2ReadyRpc(true);
        }
    }
    
}
