using UnityEngine;

public class PlayTimeUI : MonoBehaviour {
    public void Start() {
        GameManager.Instance.OnGameStart += GameManager_OnGameStart;
        GameManager.Instance.OnRematch += GameManager_OnRematch;
        Hide();
    }

    private void GameManager_OnGameStart(object sender, System.EventArgs e) {
        Show();
    }

    private void GameManager_OnRematch(object sender, System.EventArgs e) {
        Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
