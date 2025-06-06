using UnityEngine;

public class GamePosition : MonoBehaviour
{
    public int x;
    public int y;
    public bool hasBeenShot = false;
    public bool isOccupied = false;
    public IBoat boatOnPosition = null;

    private void OnMouseDown()
    {
        var playerType = GameManager.Instance.GetLocalPlayerType();
        var attackMode = InputManager.Instance.currentAttackMode;

        switch (attackMode)
        {
            case InputManager.AttackMode.Single:
                GameManager.Instance.OnClickGamePositionRpc(x, y, playerType);
                break;
            case InputManager.AttackMode.Area2x2:
                GameManager.Instance.OnClickArea2x2Rpc(x, y, playerType);
                break;
            case InputManager.AttackMode.X:
                GameManager.Instance.OnClickDiagonalXRpc(x, y, playerType);
                break;
        }
    }


    public void SetPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetHasBeenShot(bool hasBeenShot)
    {
        this.hasBeenShot = hasBeenShot;
    }

    public Vector2Int GetGridPosition()
    {
        return new Vector2Int(x, y);
    }

}
