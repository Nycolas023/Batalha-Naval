using System;
using TMPro;
using UnityEngine;

public class GamaOverUIManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI resultTextMesh;
    [SerializeField] private Color WinColor;
    [SerializeField] private Color LoseColor;

    private void Start() {
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
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

    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
}
