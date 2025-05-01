using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour {
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime = 0f;

    void Update() {
        if (remainingTime > 0f) {
            remainingTime -= Time.deltaTime;
        } else {
            remainingTime = 0f;
            timerText.outlineColor = Color.black;
            timerText.faceColor = Color.red;
        }

        int minutes = Mathf.Max(0, Mathf.FloorToInt(remainingTime / 60));
        int seconds = Mathf.Max(0, Mathf.FloorToInt(remainingTime % 60));

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}