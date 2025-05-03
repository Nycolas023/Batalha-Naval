using TMPro;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour {
    [SerializeField] TextMeshProUGUI player1PointsText;

    void Update() {
        player1PointsText.text = GameManager.Instance.boatPointsPlayer1.ToString() + "/" + GameManager.MAX_BOAT_POINTS.ToString();
    }
}
