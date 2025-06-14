using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour {
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime = 0f;
    [SerializeField] float ROUND_DURATION = 20f;

    public bool IsTimerRunning = false;

    private void Start() {
        GameManager.Instance.OnChangePlayablePlayerType += GameManager_OnChangePlayablePlayerType;

        StartTimer(ROUND_DURATION);
    }

    void Update() {
        if (!IsTimerRunning) return;

        if (remainingTime > 0f) {
            remainingTime -= Time.deltaTime;
        } else {
            remainingTime = 0f;
            timerText.outlineColor = Color.black;
            timerText.faceColor = Color.red;
            IsTimerRunning = false;
            if(GameManager.Instance.IsServer)
                GameManager.Instance.LostTurnRpc();
        }

        int minutes = Mathf.Max(0, Mathf.CeilToInt(remainingTime / 60));
        int seconds = Mathf.Max(0, Mathf.CeilToInt(remainingTime % 60));

        timerText.text = string.Format("{1:00}", minutes, seconds);
    }

    private void GameManager_OnChangePlayablePlayerType(object sender, GameManager.PlayerTypeEventArgs e) {
        StartTimer(ROUND_DURATION);
    }

    public void StartTimer(float time) {
        IsTimerRunning = true;
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