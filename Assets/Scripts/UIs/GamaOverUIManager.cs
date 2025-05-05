using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamaOverUIManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI resultTextMesh;
    [SerializeField] private Color WinColor;
    [SerializeField] private Color LoseColor;
    [SerializeField] private Button rematchButton;

    private void Start() {
        rematchButton.onClick.AddListener(() => GameManager.Instance.RematchRpc());
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
        GameManager.Instance.OnRematch += GameManager_OnRematch;
        Hide();
    }

    private void GameManager_OnGameWin(object sender, GameManager.PlayerTypeEventArgs e) {
        if (e.playerType == GameManager.Instance.GetLocalPlayerType()) {
            resultTextMesh.text = "YOU WIN!";
            resultTextMesh.color = WinColor;
        } else {
            resultTextMesh.text = "YOU LOSE!";
            resultTextMesh.color = LoseColor;
        }
        Show();
    }

    private void GameManager_OnRematch(object sender, EventArgs e) {
        Hide();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
