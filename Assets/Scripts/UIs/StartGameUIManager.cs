using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartGameUIManager : MonoBehaviour {

    [SerializeField] private Button startGameButton;
    [SerializeField] private TextMeshProUGUI readyPlayer1Text;
    [SerializeField] private TextMeshProUGUI readyPlayer2Text;

    private void Start() {
        startGameButton.onClick.AddListener(OnStartGameButtonClicked);
        GameManager.Instance.isPlayer1Ready.OnValueChanged += GameManager_OnPlayer1ReadyChanged;
        GameManager.Instance.isPlayer2Ready.OnValueChanged += GameManager_OnPlayer2ReadyChanged;

        GameManager.Instance.OnGameStart += GameManager_OnGameStart;
        GameManager.Instance.OnRematch += GameManager_OnRematch;

        readyPlayer1Text.color = Color.red;
        readyPlayer2Text.color = Color.red;
    }

    private void OnStartGameButtonClicked() {
        if (GameManager.Instance.isPlayer1Ready.Value && GameManager.Instance.isPlayer2Ready.Value) {
            Debug.Log("Both players are ready. Starting the game.");

        }

        if (GameManager.Instance.GetLocalPlayerType() == GameManager.PlayerType.Player1) {
            BoatDraggerManager.Instance.SetDraggingBoatActive(GameManager.Instance.isPlayer1Ready.Value);
            GameManager.Instance.SetIsPlayer1ReadyRpc(!GameManager.Instance.isPlayer1Ready.Value);
        } else {
            BoatDraggerManager.Instance.SetDraggingBoatActive(GameManager.Instance.isPlayer2Ready.Value);
            GameManager.Instance.SetIsPlayer2ReadyRpc(!GameManager.Instance.isPlayer2Ready.Value);
        }
    }

    private void GameManager_OnPlayer1ReadyChanged(bool previousValue, bool newValue) {
        if (newValue) {
            readyPlayer1Text.text = "READY";
            readyPlayer1Text.color = Color.green;
        } else {
            readyPlayer1Text.text = "NOT READY";
            readyPlayer1Text.color = Color.red;
        }
    }

    private void GameManager_OnPlayer2ReadyChanged(bool previousValue, bool newValue) {
        if (newValue) {
            readyPlayer2Text.text = "READY";
            readyPlayer2Text.color = Color.green;
        } else {
            readyPlayer2Text.text = "NOT READY";
            readyPlayer2Text.color = Color.red;
        }
    }

    private void GameManager_OnGameStart(object sender, EventArgs e) {
        Hide();
    }

    private void GameManager_OnRematch(object sender, EventArgs e) {
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }
    
    public void Hide() {
        gameObject.SetActive(false);
    }
}
