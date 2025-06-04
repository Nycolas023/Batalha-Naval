using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartGameUIManager : MonoBehaviour {

    [SerializeField] private Button startGameButton;

    [SerializeField] private GameObject player1ReadyIndicator;
    [SerializeField] private GameObject player2ReadyIndicator;

    private void Start() {
        startGameButton.onClick.AddListener(OnStartGameButtonClicked);
        GameManager.Instance.isPlayer1Ready.OnValueChanged += GameManager_OnPlayer1ReadyChanged;
        GameManager.Instance.isPlayer2Ready.OnValueChanged += GameManager_OnPlayer2ReadyChanged;

        GameManager.Instance.OnGameStart += GameManager_OnGameStart;
        GameManager.Instance.OnRematch += GameManager_OnRematch;

        HideReadyIndicatorPlayer1();
        HideReadyIndicatorPlayer2();
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
            ShowReadyIndicatorPlayer1();
        } else {
            HideReadyIndicatorPlayer1();
        }
    }

    private void GameManager_OnPlayer2ReadyChanged(bool previousValue, bool newValue) {
        if (newValue) {
            ShowReadyIndicatorPlayer2();
        } else {
            HideReadyIndicatorPlayer2();
        }
    }

    public void ShowReadyIndicatorPlayer1() {
        player1ReadyIndicator.transform.GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    public void ShowReadyIndicatorPlayer2() {
        player2ReadyIndicator.transform.GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    public void HideReadyIndicatorPlayer1() {
        player1ReadyIndicator.transform.GetComponent<Image>().color = new Color(1, 1, 1, 0);
    }

    public void HideReadyIndicatorPlayer2() {
        player2ReadyIndicator.transform.GetComponent<Image>().color = new Color(1, 1, 1, 0);
    }

    private void GameManager_OnGameStart(object sender, EventArgs e) {
        // Hide();
    }

    private void GameManager_OnRematch(object sender, EventArgs e) {
        // Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
