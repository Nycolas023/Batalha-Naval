using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartGameUIManager : MonoBehaviour {

    [SerializeField] private Button startGameButton;

    [SerializeField] private GameObject player1ReadyIndicator;
    [SerializeField] private GameObject player1Container;
    [SerializeField] private GameObject player2ReadyIndicator;
    [SerializeField] private GameObject player2Container;

    [SerializeField] private TextMeshProUGUI player1NameText;
    [SerializeField] private TextMeshProUGUI player2NameText;

    [SerializeField] private FlashImage flashImage;

    [SerializeField] private PlayerModelSO player;

    private void Start() {
        startGameButton.onClick.AddListener(OnStartGameButtonClicked);
        GameManager.Instance.isPlayer1Ready.OnValueChanged += GameManager_OnPlayer1ReadyChanged;
        GameManager.Instance.isPlayer2Ready.OnValueChanged += GameManager_OnPlayer2ReadyChanged;

        GameManager.Instance.OnNetworkSpawned += GameManager_OnNetworkSpawn;
        GameManager.Instance.OnGameStart += GameManager_OnGameStart;
        GameManager.Instance.OnRematch += GameManager_OnRematch;
        GameManager.Instance.OnLostTurn += GameManager_OnLostTurn;

        HideReadyIndicatorPlayer1();
        HideReadyIndicatorPlayer2();
    }

    private void Update() {
        if (GameManager.Instance.localNumberOfBoastsOnGrid < GameManager.MAX_BOATS)
            startGameButton.interactable = false;
        else
            startGameButton.interactable = true;

        if (GameManager.Instance.currentPlayablePlayerType.Value == GameManager.PlayerType.Player1) {
            player1Container.GetComponent<Image>().color = new Color(1, 1, 1, 1f);
            player2Container.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        } else if (GameManager.Instance.currentPlayablePlayerType.Value == GameManager.PlayerType.Player2) {
            player1Container.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            player2Container.GetComponent<Image>().color = new Color(1, 1, 1, 1f);
        } else {
            player1Container.GetComponent<Image>().color = new Color(1, 1, 1, 1f);
            player2Container.GetComponent<Image>().color = new Color(1, 1, 1, 1f);
        }
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

    private void GameManager_OnLostTurn(object sender, EventArgs e) {
        flashImage.StartFlash();  
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

    private void GameManager_OnNetworkSpawn(object sender, GameManager.PlayerTypeEventArgs e) {
        Invoke(nameof(SetPlayersNames), 0.5f);
    }

    private void GameManager_OnGameStart(object sender, EventArgs e) {
        // Hide();
    }

    private void GameManager_OnRematch(object sender, EventArgs e) {
        // Show();
    }

    private void SetPlayersNames() {
        player1NameText.text = GameManager.Instance.Player1Name.Value.ToString();
        player2NameText.text = GameManager.Instance.Player2Name.Value.ToString();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
