using TMPro;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour {
    [SerializeField] TextMeshProUGUI player1PointsText;

    void Update() {
        player1PointsText.text = GameManager.Instance.localPlayerBoats.Count.ToString() + "/" + GameManager.MAX_BOATS_SPAWNED.ToString();
    }
}
