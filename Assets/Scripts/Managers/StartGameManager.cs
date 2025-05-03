using UnityEngine;
using UnityEngine.UI;

public class StartGameManager : MonoBehaviour
{
    [SerializeField] private Button startGameButton;

    private void Start() {
        startGameButton.onClick.AddListener(OnStartGameButtonClicked);
    }

    private void OnStartGameButtonClicked() {
        Debug.Log("Game Started!");

        BoatDraggerManager.Instance.SetDraggingBoatActive(false);
    }
    
}
