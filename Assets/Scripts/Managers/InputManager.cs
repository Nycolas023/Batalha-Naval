using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    public static InputManager Instance { get; private set; }

    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private LayerMask defaultLayerMask;
    [SerializeField] private LayerMask gridLayerMask;
    [SerializeField] private LayerMask boatLayerMask;

    private Vector3 lastMousePosition;

    [SerializeField] private Material previewMaterial;

private GamePosition lastHoveredCell;
private Material lastHoveredOriginalMaterial;
private bool isGameStarted = false;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one InputManager instance!");
        }
        Instance = this;
        GameManager.Instance.OnGameStart += OnGameStart;
    }
    
    private void OnGameStart(object sender, EventArgs e) {
    isGameStarted = true;
}


private void Update()
    {
         if (!isGameStarted) return;
        Ray ray = GetRaycastHit();
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, defaultLayerMask)) {
    if (hit.collider.CompareTag("GridCell")) {
        GamePosition cell = hit.collider.GetComponent<GamePosition>();

        if (cell != null && cell != lastHoveredCell) {
            ClearLastPreview();

            Renderer renderer = cell.GetComponent<Renderer>();
            lastHoveredOriginalMaterial = renderer.sharedMaterial;
            renderer.sharedMaterial = previewMaterial;

            lastHoveredCell = cell;

            Debug.Log("Preview sobre: " + cell.name);
        }
    }
} else {
    ClearLastPreview();
}

    }


private void ClearLastPreview() {
    if (lastHoveredCell != null) {
        Renderer renderer = lastHoveredCell.GetComponent<Renderer>();
        renderer.sharedMaterial = lastHoveredOriginalMaterial;
        lastHoveredCell = null;
    }
}


    public Ray GetRaycastHit()
    {
        return mainCamera.ScreenPointToRay(Input.mousePosition);
    }

    public Grid GetGridBeaingIntercepted(out RaycastHit hit)
    {
        Ray ray = GetRaycastHit();
        bool didRaycastHitSomething = Physics.Raycast(ray, out hit, Mathf.Infinity, gridLayerMask);
        if (didRaycastHitSomething)
        {
            return hit.collider.GetComponent<Grid>();
        }
        return null;
    }

    public IBoat GetBoatBeaingIntercepted(out RaycastHit hit)
    {
        Ray ray = GetRaycastHit();
        bool didRaycastHitSomething = Physics.Raycast(ray, out hit, Mathf.Infinity, boatLayerMask);
        if (didRaycastHitSomething)
        {
            return hit.collider.GetComponent(typeof(IBoat)) as IBoat;
        }
        return null;
    }

    public GameObject GetGameObjectBeaingIntercepted(out RaycastHit hit)
    {
        Ray ray = GetRaycastHit();
        bool didRaycastHitSomething = Physics.Raycast(ray, out hit, Mathf.Infinity, defaultLayerMask);
        if (didRaycastHitSomething)
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    public bool IsRayHittingBoat(out RaycastHit hit)
    {
        Ray ray = GetRaycastHit();
        return Physics.Raycast(ray, out hit, Mathf.Infinity, boatLayerMask);
    }
}
