using UnityEngine;

public class GamePosition : MonoBehaviour {
    public int x;
    public int y;
    public bool hasBeenShot = false;
    public bool isOccupied = false;
    public IBoat boatOnPosition = null;

    private void OnMouseDown() {
        GameManager.Instance.OnClickGamePositionRpc(x, y, GameManager.Instance.GetLocalPlayerType());
    }

    public void SetPosition(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public void SetHasBeenShot(bool hasBeenShot) {
        this.hasBeenShot = hasBeenShot;
    }
}
