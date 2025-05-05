using System;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    [SerializeField] private Camera mainCamera;
    [SerializeField] float cameraSpeed;

    private float sinTime;
    private Vector3 targetPosition;

    private Vector3 centerCameraPosition = new Vector3(0, 25, -5);
    private Vector3 player1CameraPosition;
    private Vector3 player2CameraPosition;

    private void Awake() {
        var gridXPosition = GameManager.GRID_WIDTH * GameManager.CELL_SIZE / 2f + GameManager.GRIDS_DISTANCE / 2f;
        player1CameraPosition = new Vector3(gridXPosition, 12, -0.8f);
        player2CameraPosition = new Vector3(-gridXPosition, 12, -0.8f);

        mainCamera.transform.position = centerCameraPosition;

        mainCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    private void Start() {
        GameManager.Instance.OnChangePlayablePlayerType += GamaManager_OnChangePlayablePlayerType;
        GameManager.Instance.OnNetworkSpawned += GamaManager_OnNetworkSpawned;

        if (GameManager.Instance.GetLocalPlayerType() == GameManager.PlayerType.Player1) {
            targetPosition = player1CameraPosition;
        } else if (GameManager.Instance.GetLocalPlayerType() == GameManager.PlayerType.Player2) {
            targetPosition = player2CameraPosition;
        } else {
            targetPosition = centerCameraPosition;
        }
    }

    private void Update() {
        if (mainCamera.transform.position != targetPosition) {
            sinTime += Time.deltaTime * cameraSpeed;
            sinTime = Mathf.Clamp(sinTime, 0, (float)Math.PI);
            float t = Evaluate(sinTime);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, t);
        }
    }

    private void GamaManager_OnNetworkSpawned(object sender, GameManager.PlayerTypeEventArgs e) {
        if (e.playerType == GameManager.PlayerType.Player1) {
            targetPosition = player1CameraPosition;
        } else if (e.playerType == GameManager.PlayerType.Player2) {
            targetPosition = player2CameraPosition;
        } else {
            targetPosition = centerCameraPosition;
        }
    }

    private void GamaManager_OnChangePlayablePlayerType(object sender, GameManager.PlayerTypeEventArgs e) {
        if (e.playerType == GameManager.PlayerType.Player1) {
            targetPosition = player2CameraPosition;
        } else if (e.playerType == GameManager.PlayerType.Player2) {
            targetPosition = player1CameraPosition;
        } else {
            targetPosition = centerCameraPosition;
        }
    }

    private float Evaluate(float x) {
        return 0.5f * Mathf.Sin(x - Mathf.PI / 2f) + 0.5f;
    }
}
