using System;
using UnityEngine;

public class CameraManager : MonoBehaviour {
    [SerializeField] private Camera mainCamera;
    [SerializeField] float cameraSpeed;

    private float sinTime;
    private Vector3 targetPosition;

    private void Awake() {
        targetPosition = new Vector3(0, 35, -5);
        targetPosition = new Vector3(17.5f, 15, -5);
        mainCamera.transform.position = new Vector3(0, 35, -5);
        mainCamera.transform.position = new Vector3(17.5f, 15, -5);
        mainCamera.transform.rotation = Quaternion.Euler(78, 0, 0);
    }

    private void Start() {
        GamaManager.Instance.OnChangePlayeblePlayerType += GamaManager_OnChangePlayablePlayerType;
    }

    private void Update() {
        if (mainCamera.transform.position != targetPosition) {
            sinTime += Time.deltaTime * cameraSpeed;
            sinTime = Mathf.Clamp(sinTime, 0, (float)Math.PI);
            float t = Evaluate(sinTime);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, t);
        }
    }

    private void GamaManager_OnChangePlayablePlayerType(object sender, GamaManager.OnChangePlayeblePlayerTypeEventArgs e) {
        var gridXPosition = GamaManager.GRID_SIZE * GamaManager.CELL_SIZE / 2f + GamaManager.GRIDS_DISTANCE / 2f;
        if (e.playerType == GamaManager.Player.Player1) {
            targetPosition = new Vector3(gridXPosition, 15, -5);
        } else if (e.playerType == GamaManager.Player.Player2) {
            targetPosition = new Vector3(-gridXPosition, 15, -5);
        } else {
            targetPosition = new Vector3(0, 35, -5);
        }
    }

    private float Evaluate(float x) {
        return 0.5f * Mathf.Sin(x - Mathf.PI / 2f) + 0.5f;
    }
}
