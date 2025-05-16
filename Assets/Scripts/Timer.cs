using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour {
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime = 0f;
    [SerializeField] float ROUND_DURATION = 20f;

    public event EventHandler OnTimerEnd;

    private void Start() {
        GameManager.Instance.OnGameStart += GameManager_OnGameStart;
        GameManager.Instance.OnChangePlayablePlayerType += GameManager_OnChangePlayablePlayerType;
        Hide();
    }

    void Update() {
        if (remainingTime > 0f) {
            remainingTime -= Time.deltaTime;
        } else {
            remainingTime = 0f;
            timerText.outlineColor = Color.black;
            timerText.faceColor = Color.red;
            OnTimerEnd?.Invoke(this, EventArgs.Empty);
        }

        int minutes = Mathf.Max(0, Mathf.FloorToInt(remainingTime / 60));
        int seconds = Mathf.Max(0, Mathf.FloorToInt(remainingTime % 60));

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void GameManager_OnGameStart(object sender, EventArgs e) {
        StartTimer(ROUND_DURATION);
        Show();
    }

    private void GameManager_OnChangePlayablePlayerType(object sender, GameManager.PlayerTypeEventArgs e) {
        StartTimer(ROUND_DURATION);
        Show();
    }

    public void StartTimer(float time) {
        remainingTime = time;
        timerText.outlineColor = Color.white;
        timerText.faceColor = Color.white;
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    public void Show() {
        gameObject.SetActive(true);
    }
}