using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamaOverUIManager : MonoBehaviour {
    
    [SerializeField] private Button rematchButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private GameObject winImage;
    [SerializeField] private GameObject loseImage;

    private void Start() {
        rematchButton.onClick.AddListener(() => GameManager.Instance.RematchRpc());
        GameManager.Instance.OnGameWin += GameManager_OnGameWin;
        GameManager.Instance.OnRematch += GameManager_OnRematch;
        Hide();
    }

    private void GameManager_OnGameWin(object sender, GameManager.PlayerTypeEventArgs e) {
        if (e.playerType == GameManager.Instance.GetLocalPlayerType()) {
            winImage.SetActive(true);
            loseImage.SetActive(false);
        } else {
            winImage.SetActive(false);
            loseImage.SetActive(true);
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
