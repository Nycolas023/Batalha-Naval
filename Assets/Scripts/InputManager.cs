using System;
using UnityEngine;

public class InputManager : MonoBehaviour {

    public static InputManager Instance { get; private set; }

    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private LayerMask defaultLayerMask;
    [SerializeField] private LayerMask gridLayerMask;
    [SerializeField] private LayerMask boatLayerMask;

    private Vector3 lastMousePosition;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("More than one InputManager instance!");
        }
        Instance = this;
    }

    private void Update() {
        // Vector3 mousePos = Input.mousePosition;
        // mousePos.z = mainCamera.nearClipPlane;
        // Ray ray = mainCamera.ScreenPointToRay(mousePos);
        // RaycastHit hit;

        // if (Physics.Raycast(ray, out hit, 100, layerMask)) {
        //     lastMousePosition = hit.point;
        //     try {
        //         GamePosition gamePosition = hit.collider.GetComponent<GamePosition>();
        //         Grid grid = gamePosition.transform.parent.GetComponent<Grid>();
        //         Vector3Int gridPosition = grid.WorldToCell(hit.point);
        //         // mouseIndicator.transform.position = grid.GetCellCenterWorld(gridPosition);
        //     } catch (System.Exception e) {
        //         // Debug.Log(e.Message);
        //     }
        // }
    }

    public Ray GetRaycastHit() {
        return mainCamera.ScreenPointToRay(Input.mousePosition);
    }

    public Grid GetGridBeaingIntercepted(out RaycastHit hit) {
        Ray ray = GetRaycastHit();
        bool didRaycastHitSomething = Physics.Raycast(ray, out hit, Mathf.Infinity, gridLayerMask);
        if (didRaycastHitSomething) {
            return hit.collider.GetComponent<Grid>();
        }
        return null;
    }

    public IBoat GetBoatBeaingIntercepted(out RaycastHit hit) {
        Ray ray = GetRaycastHit();
        bool didRaycastHitSomething = Physics.Raycast(ray, out hit, Mathf.Infinity, boatLayerMask);
        if (didRaycastHitSomething) {
            return hit.collider.GetComponent(typeof(IBoat)) as IBoat;
        }
        return null;
    }

    public GameObject GetGameObjectBeaingIntercepted(out RaycastHit hit) {
        Ray ray = GetRaycastHit();
        bool didRaycastHitSomething = Physics.Raycast(ray, out hit, Mathf.Infinity, defaultLayerMask);
        if (didRaycastHitSomething) {
            return hit.collider.gameObject;
        }
        return null;
    }

    public bool IsRayHittingBoat(out RaycastHit hit) {
        Ray ray = GetRaycastHit();
        return Physics.Raycast(ray, out hit, Mathf.Infinity, boatLayerMask);
    }
}
