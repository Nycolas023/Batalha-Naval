using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;  // 👈 Isso aqui é o que resolve o erro

public class GamePosition : MonoBehaviour {
    public int x;
    public int y;
    public bool hasBeenShot = false;
    public bool isOccupied = false;
    public IBoat boatOnPosition = null;

    [SerializeField] private GameObject shotIndicator;

    private void OnMouseDown() {
        var playerType = GameManager.Instance.GetLocalPlayerType();
        var attackMode = InputManager.Instance.currentAttackMode;
        var selectedBomb = BombSelectorUI.Instance.GetCurrentBombData();

        if (playerType != GameManager.Instance.currentPlayablePlayerType.Value) {
            Debug.Log("Not your turn!");
            return;
        }

        // ✅ Bloqueia se for bomba especial e o jogador não tiver quantidade
        if ((attackMode == InputManager.AttackMode.Area2x2 || attackMode == InputManager.AttackMode.X) && selectedBomb.storedQuantity <= 0) {
            Debug.LogWarning("⛔ Sem bombas disponíveis desse tipo!");
            BombSelectorUI.Instance.FlashBombQuantityWarning();

            return;
        }

        switch (attackMode) {
            case InputManager.AttackMode.Single:
                GameManager.Instance.OnClickGamePositionRpc(x, y, playerType);
                break;
            case InputManager.AttackMode.Area2x2:
                GameManager.Instance.OnClickArea2x2Rpc(x, y, playerType);
                _ = ConsumeBombLocal(attackMode);
                break;
            case InputManager.AttackMode.X:
                GameManager.Instance.OnClickDiagonalXRpc(x, y, playerType);
                _ = ConsumeBombLocal(attackMode);
                break;
        }
    }

    public void SetPosition(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public void SetHasBeenShot(bool hasBeenShot) {
        this.hasBeenShot = hasBeenShot;
    }

    public void ShowShotIndicator() {
        if (hasBeenShot && isOccupied) shotIndicator.SetActive(true);
    }

    public void HideShotIndicator() {
        shotIndicator.SetActive(false);
    }

    public Vector2Int GetGridPosition() {
        return new Vector2Int(x, y);
    }

    private async Task ConsumeBombLocal(InputManager.AttackMode attackMode) {
        int bombId = attackMode == InputManager.AttackMode.Area2x2 ? 1 : 2;
        var playerType = GameManager.Instance.GetLocalPlayerType();
        int userId = GameManager.Instance.playerModel.Value.User_Id;
        var api = new Api();
        await api.UseBomb(userId, bombId);
        BombSelectorUI.Instance.LoadBombsForUser(userId);
    }
}
